using eCommerce.Core.DTO;
using FluentValidation;

namespace eCommerce.Core.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            //Email validation
            RuleFor(temp => temp.Email).NotEmpty()
                                       .WithMessage("Email is required")
                                       .EmailAddress()
                                       .WithMessage("Invalid email");

            //Password validation

            RuleFor(temp => temp.Password).NotEmpty()
                .WithMessage("Password is required");

            
        }
    }
}
