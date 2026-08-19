using AITextSummarizer.Core.Models;
using FluentAssertions;
using AITextSummarizer.Api.Validators;

namespace AITextSummarizer.Tests.UnitTests;

public class SummarizeRequestValidatorTests
{
    private readonly SummarizeRequestValidator _validator = new();

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        var request = new SummarizeRequest
        {
            Text = "A short but valid piece of text to summarize.",
            MaxLength = 100,
            Format = SummaryFormat.Paragraph
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyText_FailsValidation(string? text)
    {
        var request = new SummarizeRequest
        {
            Text = text!,
            MaxLength = 100,
            Format = SummaryFormat.Paragraph
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SummarizeRequest.Text));
    }

    [Fact]
    public void TextTooLong_FailsValidation()
    {
        var request = new SummarizeRequest
        {
            Text = new string('a', 10001),
            MaxLength = 100,
            Format = SummaryFormat.Paragraph
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SummarizeRequest.Text));
    }

    [Theory]
    [InlineData(19)]
    [InlineData(501)]
    public void MaxLengthOutOfRange_FailsValidation(int maxLength)
    {
        var request = new SummarizeRequest
        {
            Text = "Valid text here.",
            MaxLength = maxLength,
            Format = SummaryFormat.Paragraph
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SummarizeRequest.MaxLength));
    }
}