using System.Text.Json;
using AITextSummarizer.Core.Interfaces;
using AITextSummarizer.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AITextSummarizer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SummarizationController : ControllerBase
{
    private readonly ISummarizationService _service;

    public SummarizationController(ISummarizationService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<SummarizeResponse>> Summarize(
        [FromBody] SummarizeRequest request, CancellationToken ct)
        => Ok(await _service.SummarizeAsync(request, ct));

    [HttpPost("stream")]
    public async Task Stream([FromBody] SummarizeRequest request, CancellationToken ct)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        await Response.StartAsync(ct);

        await foreach (var chunk in _service.SummarizeStreamAsync(request, ct))
        {
            var payload = JsonSerializer.Serialize(new { content = chunk });
            await Response.WriteAsync($"data: {payload}\n\n", ct);
            await Response.Body.FlushAsync(ct);
        }

        await Response.WriteAsync("data: [DONE]\n\n", ct);
    }
}