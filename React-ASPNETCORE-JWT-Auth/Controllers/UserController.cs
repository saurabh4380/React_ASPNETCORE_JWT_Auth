using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using React_ASPNETCORE_JWT_Auth.Models;
using React_ASPNETCORE_JWT_Auth.Services;

namespace React_ASPNETCORE_JWT_Auth.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        //1) Handle logins

        [HttpPost, Route("login")]
        public ActionResult<UserLoginResponseModel> Login([FromBody] UserLoginRequestModel userLoginRequestModel)
        {

            var response = _userService.HandleUserLogin(userLoginRequestModel);

            SetTokenCookie(response.RefreshToken);

            return Ok(response);
            
        }

        //2) Handle new user SignUp
        [HttpPost, Route("signup")]
        public ActionResult<UserSignupResponseModel> SignUp([FromBody] UserSignupRequestModel userSignupRequestModel)
        {

            var response = _userService.HandleUserSignUp(userSignupRequestModel);

            SetTokenCookie(response.RefreshToken);

            return Ok(response);
           
        }

        //2) Handle new user SignUp
        [HttpPost, Route("refreshtoken")]
        public ActionResult<UserLoginResponseModel> RefreshToken()
        {

            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest();
            }

            var response = _userService.GetTokens(refreshToken);

            SetTokenCookie(response.RefreshToken);

            return Ok(response);

        }

        [HttpPost("reset-password/{passwordResetLink}")]
        public ActionResult<PasswordResetResponseModel> ResetPassword(string passwordResetLink, [FromBody]PasswordResetRequestModel passwordResetRequestModel)
        {

            var badResponse = new BaseResponseModel();

            if (Guid.TryParse(passwordResetLink, out _))
            {
                var response = _userService.HandlePasswordReset(passwordResetLink, passwordResetRequestModel);

                return Ok(response);

            }

            badResponse.Errors = new List<string>() { "Invalid URL" };

            return BadRequest(badResponse);
           
        }

        [HttpPost, Route("generateResetPassword")]
        public ActionResult<PasswordResetResponseModel> GenerateResetPassword(string userEmail)
        {
            var response = _userService.GenerateAndSavePasswordResetLink(userEmail);

            return Ok(response);
        }

        [HttpGet, Route("test-error")]
        public IActionResult TestError()
        {
            throw new NotImplementedException();

            return BadRequest();
        }

        /// <summary>
        /// Utility to set a refreshToken as HttpOnly cookie with a validity of 7 days on the response.
        /// </summary>
        /// <param name="refreshToken"></param>
        private void SetTokenCookie(string refreshToken)
        {
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                // append cookie with refresh token to the http response
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(7)
                };
                Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
            }
           
        }
    }
}
