using Microsoft.Extensions.Resilience;
using Polly.Retry;
using Polly.CircuitBreaker;
using Polly.Timeout;
using Polly;
using AITextSummarizer.Api.Health;
using AITextSummarizer.Core.Interfaces;
using AITextSummarizer.Infrastructure.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.SemanticKernel;
using Scalar.AspNetCore;
using Serilog;
using System.Threading.RateLimiting;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024;
});

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddFluentValidationAutoValidation();

builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection(OllamaOptions.SectionName));

var ollama = builder.Configuration.GetSection(OllamaOptions.SectionName).Get<OllamaOptions>() ?? new OllamaOptions();

builder.Services.AddKernel();
builder.Services.AddOpenAIChatCompletion(
    modelId: ollama.ModelName,
    endpoint: new Uri(ollama.Endpoint),
    apiKey: "ollama"
);

builder.Services.AddScoped<ISummarizationService, OllamaSummarizationService>();
builder.Services.AddResiliencePipeline("ollama-summarize", pipeline =>
{
    pipeline.AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(2),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true
    })
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        MinimumThroughput = 5,
        FailureRatio = 0.5,
        SamplingDuration = TimeSpan.FromSeconds(30),
        BreakDuration = TimeSpan.FromSeconds(30)
    }).AddTimeout(TimeSpan.FromSeconds(120));
});


builder.Services.AddHttpClient<OllamaHealthCheck>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434");
});
builder.Services.AddHealthChecks()
    .AddCheck<OllamaHealthCheck>("ollama");

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("per-ip", httpContext =>
    RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown", factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 30,
        Window = TimeSpan.FromMinutes(1),
        QueueLimit = 2
    })
    );
});

var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection();
app.MapControllers();
app.UseRateLimiter();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Name == "ollama"
});
app.Run();