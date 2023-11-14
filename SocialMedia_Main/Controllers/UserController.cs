using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SocialMedia_Common.UserView;
using SocialMedia_Data.Entity;
using SocialMedia_DataAccess.Repository;
using SocialMedia_Services.UsersService;
using System.Net.Http;

namespace SocialMedia_Main.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public UserController(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("getUserDetails")]
        public async Task<IActionResult> GetUserDetails(string userId)
        {
            var user = await _userService.GetUserById(userId);
            if(user == null)
            {
                return Ok(new { IsSuccess = true, Data = user, ErrorMessage = $"User not found for userId: {userId}" });
            }
            return Ok(new { IsSuccess = true, Data = user, ErrorMessage = $"" });
        }
        [HttpGet]
        [Route("getAllUsers")]
        public async Task<IActionResult> GetAllUsers(string userId, int pageNumber, int? pageSize)
        {
            pageSize ??= 10;
            var user = await _userService.GetAllRequestUsers(userId, pageNumber, (int)pageSize);
            return Ok(new { IsSuccess = true, Data = user, ErrorMessage = $"" });
        }
        [HttpPost]
        [Route("sendFriendRequest")]
        public async Task<IActionResult> SendFriendRequest(Guid senderId, Guid receiverId)
        {
            var loginUserId = await GetUserIdAsync(this._userManager);
            var request = await _userService.SendFriendRequest(Guid.Parse(loginUserId), receiverId);
            if(request == null)
            {
                return Ok(new { IsSuccess = true, Data = request, ErrorMessage = $"User does not exist for userId: {receiverId}" });
            }
            return Ok(new { IsSuccess = true, Data = request, ErrorMessage = $"" });
        }

        [HttpGet]
        [Route("getSearchUser")]
        public async Task<IActionResult> GetSearchUser(string searchString)
        {
            var user = await _userService.GetSearchUser(searchString);
            if(user == null)
            {
                return Ok(new { IsSuccess = true, Data = user, ErrorMessage = $"No user for the searchString: {searchString}" });
            }
            return Ok(new { IsSuccess = true, Data = user, ErrorMessage = $"" });
        }

        [HttpPost]
        [Route("editUserDetails")]
        public async Task<IActionResult> EditUserDetails(EditUserDetailsViewModel editUserDetails)
        {
            var loginUserId = await GetUserIdAsync(this._userManager);
            var user = await _userService.EditUserDetails(editUserDetails, loginUserId);
            return Ok(new { IsSuccess = true, Data = user, ErrorMessage = $"" });
        }

    }
}
