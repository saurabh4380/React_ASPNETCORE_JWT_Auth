using React_ASPNETCORE_JWT_Auth.Authentication;
using React_ASPNETCORE_JWT_Auth.Services;

namespace React_ASPNETCORE_JWT_Auth.Middlewares
{
    /// <summary>
    /// A middleware used to Validate the JWT Token from the incoming request (Authentication). 
    /// If the token is valid then it will parse the UserId from the Token and grab that User from DB and attach the UserInfo on the HttpContext. 
    /// The attached UserInfo will be used by the [Authorize] attribute for Authorization.
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var userId = jwtUtils.ValidateJwtToken(token ?? "");

            if (userId != null)
            {
                // attach user to context on successful jwt validation

                var user = userService.GetById(Guid.Parse(userId));

                if (user != null)
                {
                    context.Items["User"] = user;
                }
            }

            await _next(context);
        }
    }
}
