using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using AITextSummarizer.Core.Interfaces;
using AITextSummarizer.Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AITextSummarizer.Tests.IntegrationTests;

public class SummarizationApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SummarizationApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<ISummarizationService>(_ => new StubSummarizationService());
            });
        });
    }

    [Fact]
    public async Task Summarize_ValidRequest_ReturnsSummary()
    {
        var client = _factory.CreateClient();
        var request = new SummarizeRequest
        {
            Text = "This is a short piece of text that needs summarizing.",
            MaxLength = 50,
            Format = SummaryFormat.Paragraph
        };

        var response = await client.PostAsJsonAsync("/api/summarization", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<SummarizeResponse>();
        body.Should().NotBeNull();
        body!.Summary.Should().Be("Stubbed summary result.");
        body.Model.Should().Be("stub");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Summarize_EmptyText_ReturnsValidationError(string text)
    {
        var client = _factory.CreateClient();
        var request = new SummarizeRequest
        {
            Text = text,
            MaxLength = 50,
            Format = SummaryFormat.Paragraph
        };

        var response = await client.PostAsJsonAsync("/api/summarization", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Summarize_TextTooLong_ReturnsValidationError()
    {
        var client = _factory.CreateClient();
        var request = new SummarizeRequest
        {
            Text = new string('a', 10001),
            MaxLength = 50,
            Format = SummaryFormat.Paragraph
        };

        var response = await client.PostAsJsonAsync("/api/summarization", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Summarize_BodyTooLarge_ReturnsRequestEntityTooLarge()
    {
        var client = _factory.CreateClient();
        var json = $"{{\"text\":\"{new string('a', 60 * 1024)}\",\"maxLength\":50,\"format\":0}}";

        var response = await client.PostAsync("/api/summarization",
            new StringContent(json, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.RequestEntityTooLarge);
    }

    [Fact]
    public async Task Stream_ReturnsServerSentEvents()
    {
        var client = _factory.CreateClient();
        var request = new SummarizeRequest
        {
            Text = "Some text to stream through the endpoint.",
            MaxLength = 50,
            Format = SummaryFormat.Paragraph
        };

        var response = await client.PostAsJsonAsync("/api/summarization/stream", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/event-stream");
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("data:");
        body.Should().Contain("[DONE]");
    }
}

file class StubSummarizationService : ISummarizationService
{
    public Task<SummarizeResponse> SummarizeAsync(SummarizeRequest request, CancellationToken ct = default)
        => Task.FromResult(new SummarizeResponse
        {
            Summary = "Stubbed summary result.",
            Model = "stub",
            OriginalWordCount = 0,
            SummaryWordCount = 4,
            ProcessingTimeMs = 1
        });

    public async IAsyncEnumerable<string> SummarizeStreamAsync(
        SummarizeRequest request, [EnumeratorCancellation] CancellationToken ct = default)
    {
        yield return "Stubbed ";
        await Task.Yield();
        yield return "summary ";
        await Task.Yield();
        yield return "result.";
    }
}