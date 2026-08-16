using AITextSummarizer.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using AITextSummarizer.Core.Models;
using System.Diagnostics;

namespace AITextSummarizer.Infrastructure.Services;

public class OllamaSummarizationService : ISummarizationService
{
    private readonly IChatCompletionService _chatCompletion;
    private readonly ILogger<OllamaSummarizationService> _logger;
    private readonly OllamaOptions _options;

    public OllamaSummarizationService(Kernel kernel, IOptions<OllamaOptions> options, ILogger<OllamaSummarizationService> logger)
    {
        _chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
        _options = options.Value;
        _logger = logger;
    }

    public async Task<SummarizeResponse> SummarizeAsync(SummarizeRequest request, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var reply = await _chatCompletion.GetChatMessageContentAsync(
            new ChatHistory(BuildPrompt(request)), cancellationToken: ct);

        sw.Stop();
        return new SummarizeResponse
        {
            Summary = reply.Content ?? string.Empty,
            Model = _options.ModelName,
            OriginalWordCount = CountWords(request.Text),
            SummaryWordCount = CountWords(reply.Content ?? string.Empty),
            ProcessingTimeMs = sw.ElapsedMilliseconds
        };
    }

    public async IAsyncEnumerable<string> SummarizeStreamAsync(SummarizeRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var chunk in _chatCompletion.GetStreamingChatMessageContentsAsync(
            new ChatHistory(BuildPrompt(request)), cancellationToken: ct))
        {
            if (chunk.Content is not null) yield return chunk.Content;
        }
    }

    private static string BuildPrompt(SummarizeRequest request)
    {
        var format = request.Format == SummaryFormat.Bullets
            ? "Provide the summary as bullet points."
            : "Provide the summary as a single flowing paragraph.";

        return $"""
            You are an expert summarizer. Summarize the following text in at most {request.MaxLength} words.
            {format}
            Keep only the most important information. Stay factual, preserve the original meaning.

            TEXT TO SUMMARIZE:
            {request.Text}
            """;
    }

    private static int CountWords(string text) =>
        string.IsNullOrWhiteSpace(text) ? 0 : text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
}

public class OllamaOptions
{
    public const string SectionName = "Ollama";
    public string Endpoint { get; set; } = "http://localhost:11434/";
    public string ModelName { get; set; } = "qwen3:8b";
}