using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialMedia_Data.Entity;

namespace SocialMedia_Main.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<string> GetUserIdAsync(UserManager<User> userManager)
        {
            var userClaim = User.FindFirst("jti");
            return userClaim.Value;
        }
    }
}
