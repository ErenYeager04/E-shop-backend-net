using E_shop_backend.Dtos;
using FluentValidation;

namespace E_shop_backend.Validations
{
    public class Cart_ProductDtoValidator : AbstractValidator<Cart_ProductDto>
    {
        public Cart_ProductDtoValidator()
        {
            RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product id is required");

            RuleFor(x => x.Seasons)
            .NotEmpty().WithMessage("Choose seasons");

            RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Please sign in");
        }
    }
}
