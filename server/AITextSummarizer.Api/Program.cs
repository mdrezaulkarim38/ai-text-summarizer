using AITextSummarizer.Core.Interfaces;
using AITextSummarizer.Infrastructure.Services;
using Microsoft.SemanticKernel;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection(OllamaOptions.SectionName));

var ollama = builder.Configuration.GetSection(OllamaOptions.SectionName).Get<OllamaOptions>() ?? new OllamaOptions();

builder.Services.AddKernel();
builder.Services.AddOpenAIChatCompletion(
    modelId: ollama.ModelName,
    endpoint: new Uri(ollama.Endpoint),
    apiKey: "ollama"
);

builder.Services.AddScoped<ISummarizationService, OllamaSummarizationService>();

var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();