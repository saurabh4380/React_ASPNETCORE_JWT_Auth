using FluentValidation;
using React_ASPNETCORE_JWT_Auth.Models;

namespace React_ASPNETCORE_JWT_Auth.Validators
{
    public class UserLoginRequestValidator : AbstractValidator<UserLoginRequestModel>
    {
        public UserLoginRequestValidator()
        {
            RuleFor(p => p.EmailId).NotEmpty()
                                   .EmailAddress();

            RuleFor(p => p.Password).NotEmpty();
        }
    }
}
