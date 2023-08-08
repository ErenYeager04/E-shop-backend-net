using E_shop_backend.Dtos;
using FluentValidation;

namespace E_shop_backend.Validations
{
    public class ReqProductDtoValidator : AbstractValidator<ReqProductDto>
    {
        public ReqProductDtoValidator()
        {
            RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.")
            .MaximumLength(20).WithMessage("Title cannot exceed 20 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MinimumLength(5).WithMessage("Description must be at least 5 characters.")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("ImageUrl is required.")
                .Must(BeAValidUrl).WithMessage("ImageUrl must be a valid URL.");

            RuleFor(x => x.Seasons)
                .NotEmpty().WithMessage("Seasons is required.");

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Price is required.");

            RuleFor(x => x.RatingId)
                .NotEmpty().WithMessage("Rating is required.");

            RuleFor(x => x.StudioId)
                .NotEmpty().WithMessage("Studio is required.");

            RuleFor(x => x.ProductGenres)
                .NotEmpty().WithMessage("ProductGenres is required.")
                .Must(genres => genres.All(genre => genre != 0))
                .WithMessage("ProductGenres cannot contain zero.");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    
    }
}
