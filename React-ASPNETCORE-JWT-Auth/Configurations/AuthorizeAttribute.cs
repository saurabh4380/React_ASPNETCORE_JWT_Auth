using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using React_ASPNETCORE_JWT_Auth.DataAccess.Entities;
using React_ASPNETCORE_JWT_Auth.Models;

namespace React_ASPNETCORE_JWT_Auth.Configurations
{
    /// <summary>
    /// Custom Authorize Attribute used to perform Role-Based Authorization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IList<Role> _roles;

        public AuthorizeAttribute(params Role[] roles)
        {
            _roles = roles ?? Array.Empty<Role>();
        }

        /// <summary>
        /// Authorizes the incoming request by checking the "User" and his/her "Role" properties on the HttpContext.
        /// If "User" proerty is present on the HttpContext with the specified Role then its an Authorized request. 
        /// Skips the Authorization check for the Controllers/Action Methods marked with [AllowAnonymous] attributes.
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();

            if (allowAnonymous)
            {
                return;
            }

            // authorization
            var user = context.HttpContext.Items["User"] as UserAuthModel;

            if (user == null)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

            }
            else if (_roles.Any() && !_roles.Contains(user.Role))
            {
                context.Result = new JsonResult(new { message = "Unauthorized Access" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
