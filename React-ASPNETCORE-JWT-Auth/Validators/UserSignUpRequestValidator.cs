using FluentValidation;
using React_ASPNETCORE_JWT_Auth.Models;

namespace React_ASPNETCORE_JWT_Auth.Validators
{
    public class UserSignUpRequestValidator : AbstractValidator<UserSignupRequestModel>
    {
        public UserSignUpRequestValidator()
        {
            RuleFor(p => p.FirstName).NotEmpty().MaximumLength(150);
            RuleFor(p => p.LastName).NotEmpty().MaximumLength(150);
            RuleFor(p => p.MobileNumber).NotEmpty().MaximumLength(10);

            RuleFor(p => p.EmailId).EmailAddress().WithMessage("EmailId format is invalid").NotEmpty().WithMessage("EmailId address cannot be empty");
            RuleFor(p => p.ConfirmEmailAddress).EmailAddress().WithMessage("EmailId format is invalid").NotEmpty().WithMessage("ConfirmEmailAddress cannot be empty");
            RuleFor(p => p.EmailId).Must((userSignupRequestModel, emailId) => emailId == userSignupRequestModel.ConfirmEmailAddress).WithMessage("EmailId and ConfirmEmailAddress should match");

            RuleFor(p => p.Password).NotEmpty().WithMessage("Password cannot be empty");
            RuleFor(p => p.ConfirmPassword).NotEmpty().WithMessage("ConfirmPassword cannot be empty");
            RuleFor(p => p.Password).Must((userSignupRequestModel, password) => password == userSignupRequestModel.ConfirmPassword).WithMessage("Password and ConfirmPassword should match");

        }
    }
}
