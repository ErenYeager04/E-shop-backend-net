using E_shop_backend.Dtos;
using FluentValidation;

namespace E_shop_backend.Validations
{
    public class LoginDtoValidator : AbstractValidator<LoginDto> 
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
        }
    }
}
