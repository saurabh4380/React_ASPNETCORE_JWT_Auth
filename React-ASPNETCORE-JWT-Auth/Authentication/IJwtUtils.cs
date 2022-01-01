using React_ASPNETCORE_JWT_Auth.Models;

namespace React_ASPNETCORE_JWT_Auth.Authentication
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(UserAuthModel user);
        public string ValidateJwtToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress);
    }
}