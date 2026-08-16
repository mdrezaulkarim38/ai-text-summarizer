namespace AITextSummarizer.Core.Models;

public enum SummaryFormat { Paragraph, Bullets }
public record SummarizeRequest
{
    public required string Text { get; init; }
    public int MaxLength { get; init; } = 150;
    public SummaryFormat Format { get; init; } = SummaryFormat.Paragraph;
}

public record SummarizeResponse
{
    public required string Summary { get; init; }
    public string Model { get; init; } = "";
    public int OriginalWordCount { get; init; }
    public int SummaryWordCount { get; init; }
    public long ProcessingTimeMs { get; init; }
}