namespace React_ASPNETCORE_JWT_Auth.Models
{
    public class PasswordResetResponseModel : BaseResponseModel
    {
        public string PasswordResetLink { get; set; }
        public DateTime PasswordResetExpirationTime { get; set; }
        public string UserName { get; set; }

    }
}
