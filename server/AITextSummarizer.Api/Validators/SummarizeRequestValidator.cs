using AITextSummarizer.Core.Models;
using FluentValidation;

namespace AITextSummarizer.Api.Validators;

public class SummarizeRequestValidator : AbstractValidator<SummarizeRequest>
{
    public SummarizeRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Text is required")
            .MaximumLength(10000).WithMessage("Text cannot exceed 10,000 characters");

        RuleFor(x => x.MaxLength)
            .InclusiveBetween(20, 500).WithMessage("MaxLength must be between 20 and 500");
    }
}