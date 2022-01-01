using System.Text.Json.Serialization;

namespace React_ASPNETCORE_JWT_Auth.Models
{
    public class UserLoginResponseModel : BaseResponseModel
    {
        public string AccessToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpirationTime { get; set; }

    }
}
