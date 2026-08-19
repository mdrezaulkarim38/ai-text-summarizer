using System.Runtime.CompilerServices;
using AITextSummarizer.Core.Models;
using AITextSummarizer.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Resilience;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;
using Polly.Registry;

namespace AITextSummarizer.Tests.UnitTests;

public class OllamaSummarizationServiceTests
{
    private readonly Mock<IChatCompletionService> _chatCompletion;

    public OllamaSummarizationServiceTests()
    {
        _chatCompletion = new Mock<IChatCompletionService>();
    }

    private OllamaSummarizationService CreateService()
    {
        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.Services.AddSingleton(_chatCompletion.Object);
        var kernel = kernelBuilder.Build();

        var services = new ServiceCollection();
        services.AddResiliencePipeline("ollama-summarize", pipeline => pipeline.AddTimeout(TimeSpan.FromSeconds(10)));
        var provider = services.BuildServiceProvider().GetRequiredService<ResiliencePipelineProvider<string>>();

        var options = Options.Create(new OllamaOptions
        {
            Endpoint = "http://localhost:11434/",
            ModelName = "qwen3:8b"
        });

        return new OllamaSummarizationService(
            kernel,
            options,
            NullLogger<OllamaSummarizationService>.Instance,
            provider);
    }

    [Fact]
    public async Task SummarizeAsync_ReturnsStructuredResponse()
    {
        _chatCompletion
            .Setup(x => x.GetChatMessageContentAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings?>(),
                It.IsAny<Kernel?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChatMessageContent(AuthorRole.Assistant, content: "A concise summary of the text."));

        var service = CreateService();
        var request = new SummarizeRequest
        {
            Text = "First sentence. Second sentence. Third sentence here.",
            MaxLength = 50,
            Format = SummaryFormat.Paragraph
        };

        var response = await service.SummarizeAsync(request);

        response.Summary.Should().Be("A concise summary of the text.");
        response.Model.Should().Be("qwen3:8b");
        response.OriginalWordCount.Should().Be(7);
        response.SummaryWordCount.Should().Be(7);
        response.ProcessingTimeMs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task SummarizeAsync_EmptyModelResponse_ReturnsEmptySummary()
    {
        _chatCompletion
            .Setup(x => x.GetChatMessageContentAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings?>(),
                It.IsAny<Kernel?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChatMessageContent(AuthorRole.Assistant, content: null));

        var service = CreateService();
        var request = new SummarizeRequest
        {
            Text = "Some text.",
            MaxLength = 50,
            Format = SummaryFormat.Paragraph
        };

        var response = await service.SummarizeAsync(request);

        response.Summary.Should().BeEmpty();
    }

    [Fact]
    public async Task SummarizeStreamAsync_YieldsChunks()
    {
        var chunks = new List<StreamingChatMessageContent>
        {
            new(AuthorRole.Assistant, content: "Hello "),
            new(AuthorRole.Assistant, content: "world")
        };

        _chatCompletion
            .Setup(x => x.GetStreamingChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings?>(),
                It.IsAny<Kernel?>(),
                It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(chunks));

        var service = CreateService();
        var request = new SummarizeRequest
        {
            Text = "Some text to stream.",
            MaxLength = 50,
            Format = SummaryFormat.Bullets
        };

        var result = new List<string>();
        await foreach (var chunk in service.SummarizeStreamAsync(request))
        {
            result.Add(chunk);
        }

        result.Should().Equal("Hello ", "world");
    }

    private static async IAsyncEnumerable<StreamingChatMessageContent> ToAsyncEnumerable(
        IEnumerable<StreamingChatMessageContent> items)
    {
        foreach (var item in items)
        {
            yield return item;
        }
        await Task.CompletedTask;
    }
}