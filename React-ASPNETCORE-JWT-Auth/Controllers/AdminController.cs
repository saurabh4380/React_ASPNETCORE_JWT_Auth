using Microsoft.AspNetCore.Mvc;
using React_ASPNETCORE_JWT_Auth.Configurations;
using React_ASPNETCORE_JWT_Auth.DataAccess.Entities;
using React_ASPNETCORE_JWT_Auth.Services;

namespace React_ASPNETCORE_JWT_Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(roles:Role.Admin)]
    public class AdminController : ControllerBase
    {
        private IUserService _userService;
        public AdminController(IUserService userService)
        {
            _userService = userService; 
        }

    }
}
