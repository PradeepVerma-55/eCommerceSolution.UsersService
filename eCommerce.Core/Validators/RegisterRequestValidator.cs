using eCommerce.Core.DTO;
using FluentValidation;

namespace eCommerce.Core.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {

            RuleFor(model => model.Email)

               .NotEmpty().WithMessage("email must not be empty")

               .EmailAddress().WithMessage("email must be valid");

            RuleFor(model => model.Password)
                .NotEmpty().WithMessage("password must be not empty")
                .MinimumLength(6).WithMessage("password must be atleas 6 chars")
                .MaximumLength(20).WithMessage("password max length is 20");

            RuleFor(model => model.PersonName)
                .NotEmpty()
                .WithMessage("name must be not empty");

            RuleFor(model => model.Gender)
                .IsInEnum()
                .WithMessage("must be valid value");

        }
    }
}
