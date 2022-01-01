using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using React_ASPNETCORE_JWT_Auth.Configurations;
using React_ASPNETCORE_JWT_Auth.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace React_ASPNETCORE_JWT_Auth.Authentication
{
    /// <summary>
    /// A class used to Generate JWT token and Refresh Tokens and also validate the JWT tokens.
    /// </summary>
    public class JwtUtils : IJwtUtils
    {
        private readonly AppSettings _appSettings;

        public JwtUtils(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

        }

        /// <summary>
        /// Utility used to generate the JWT token with the secret from the Appsettings file.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>A JWT token signed with a Secret, UserId as Claim and a validity of 15 mins</returns>
        public string GenerateJwtToken(UserAuthModel user)
        {
            // generate token that is valid for 15 minutes
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Used to generate the Refresh Token.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns>A Refresh Token with a validity of 7 days</returns>
        public RefreshToken GenerateRefreshToken(string ipAddress)
        {
            //TODO: Should we store ipAddress ?
            using var rngCryptoServiceProvider = new RSACryptoServiceProvider();

            var randomBytes = new byte[64];

            randomBytes = rngCryptoServiceProvider.Encrypt(randomBytes, false);

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                CreationTime = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            return refreshToken;
        }


        /// <summary>
        /// Used to validate the JWT token with the Secret Key from the Appsettings file.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>If token is valid then returns the UserID. If the Token is invalid then returns null</returns>
        public string ValidateJwtToken(string token)
        {
            if (token == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    //SaveSigninToken = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken) validatedToken;

                var userId = jwtToken.Claims.First(x => x.Type == "id").Value;

                // return user id from JWT token if validation successful
                return userId;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }
    }
}
