using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AITextSummarizer.Api.Health;

public class OllamaHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;

    public OllamaHealthCheck(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tags", cancellationToken);
            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy("Ollama is reachable")
                : HealthCheckResult.Unhealthy($"Ollama returned {(int)response.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Ollama unreachable: {ex.Message}");
        }
    }
}