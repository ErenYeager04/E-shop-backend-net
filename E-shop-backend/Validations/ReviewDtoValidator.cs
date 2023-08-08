using E_shop_backend.Dtos;
using FluentValidation;

namespace E_shop_backend.Validations
{
    public class ReviewDtoValidator : AbstractValidator<ReviewDto>
    {
        public ReviewDtoValidator()
        {
            RuleFor(x => x.Rating)
            .NotEmpty().WithMessage("Rating is required");

            RuleFor(x => x.Comment)
                .MinimumLength(10).WithMessage("Your comment is too short")
                .MaximumLength(50).WithMessage("You comment is too long");
        }
    }
}
