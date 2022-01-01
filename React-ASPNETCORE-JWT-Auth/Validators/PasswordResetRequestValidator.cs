using FluentValidation;
using React_ASPNETCORE_JWT_Auth.Models;

namespace React_ASPNETCORE_JWT_Auth.Validators
{
    public class PasswordResetRequestValidator : AbstractValidator<PasswordResetRequestModel>
    {
        public PasswordResetRequestValidator()
        {
            this.CascadeMode = CascadeMode.Stop;

            RuleFor(p => p.Password).NotEmpty().WithMessage("Password cannot be empty")
                                    .Matches(@"^(?=.*[A-Za-z])(?=.*\d).{8,}$").WithMessage("Password should have minimum eight characters, at least one letter and one number")
                                    .Must((passwordResetRequestModel, pwd) => passwordResetRequestModel.ConfirmPassword == pwd).WithMessage("Password and ConfirmPassword should match");

            RuleFor(p => p.ConfirmPassword).NotEmpty().WithMessage("ConfirmPassword cannot be empty")
                                           .Matches(@"^(?=.*[A-Za-z])(?=.*\d).{8,}$").WithMessage("Password should have minimum eight characters, at least one letter and one number")
                                           .Must((passwordResetRequestModel, pwd) => passwordResetRequestModel.Password == pwd).WithMessage("Password and ConfirmPassword should match");
        }
    }
}
