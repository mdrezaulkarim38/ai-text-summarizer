using AITextSummarizer.Core.Models;

namespace AITextSummarizer.Core.Interfaces;

public interface ISummarizationService
{
    Task<SummarizeResponse> SummarizeAsync(SummarizeRequest request, CancellationToken ct = default);
    IAsyncEnumerable<string> SummarizeStreamAsync(SummarizeRequest request, CancellationToken ct = default);
}