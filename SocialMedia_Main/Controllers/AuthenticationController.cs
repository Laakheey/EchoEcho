using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SocialMedia_Common.Authentication;
using SocialMedia_Data.Entity;
using SocialMedia_Services.Authentication;
using MailKit.Net.Smtp;
using MailKit.Security;
using Google.Apis.Auth.OAuth2.Requests;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialMedia_Main.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;
        private IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string secretKey = "MySecurityKeyForTheAuthenticationOfMySocialMediaWebApplication";

        public AuthenticationController(SignInManager<User> signInManager, UserManager<User> userManager, IAuthService authService, IConfiguration config, IWebHostEnvironment webHostEnvironment)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _authService = authService;
            _config = config;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                
                return Ok(new { IsSuccess = false, Errors = errors});
            }
            var isLoginSuccess = await _authService.Login(loginViewModel);
            return Ok(isLoginSuccess);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(SignupViewModel signupViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();

                return Ok(new { IsSuccess = false, Errors = errors });
            }
            var isLoginSuccess = await _authService.Register(signupViewModel);
            //if (isLoginSuccess.IsSuccess)
            //{
            //}
            return Ok(isLoginSuccess);
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();
            return Ok(new {IsLoggedOut = true});
        }

        [Authorize]
        [HttpPost]
        [Route("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            var email = User?.Identity?.Name;
            if (changePasswordViewModel.NewPassword == changePasswordViewModel.OldPassword)
            {
                var error = new List<string>{ "New password and old password cannot be same" };
                return Ok(new { IsSuccess = false, Errors = error, Token="", UserId="" });
            }
            if (changePasswordViewModel.NewPassword != changePasswordViewModel.ConfirmPassword)
            {
                var error = new List<string> { "New password and confirm password must be same" };
                return Ok(new { IsSuccess = false, Errors = error, Token = "", UserId="" });
            }
            var isChangePasswordSuccess = await _authService.ChangePassword(changePasswordViewModel, email);
            return Ok(isChangePasswordSuccess);
        }

        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken(string userId)
        {
            var user = await _authService.RefreshToken(userId);
            return Ok(new {IsSuccess = true, Token = user});
        }

        [HttpGet("getCountries")]
        public async Task<IActionResult> GetCountries()
        {
            //var countries = await
            return Ok();
        }

    }
}
