using React_ASPNETCORE_JWT_Auth.Models;

namespace React_ASPNETCORE_JWT_Auth.Services
{
    public interface IUserService
    {
        UserAuthModel GetById(Guid id);
        UserLoginResponseModel HandleUserLogin(UserLoginRequestModel userLoginRequestModel);
        UserSignupResponseModel HandleUserSignUp(UserSignupRequestModel userSignupRequestModel);
        UserLoginResponseModel GetTokens(string refreshToken);
        public PasswordResetResponseModel HandlePasswordReset(string passwordResetLink, PasswordResetRequestModel passwordResetRequestModel);
        public PasswordResetResponseModel GenerateAndSavePasswordResetLink(string userEmail);
    }
}
