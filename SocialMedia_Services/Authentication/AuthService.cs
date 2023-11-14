using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
//using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;
using SocialMedia_Common.Authentication;
using SocialMedia_Data;
using SocialMedia_Data.Entity;
using SocialMedia_DataAccess.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace SocialMedia_Services.Authentication
{
    public class AuthService : IAuthService
    {
        private IConfiguration _config;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly SocialMediaDataContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly IGenericRepository<User> _userRepository;
        public AuthService(IConfiguration config, IGenericRepository<User> userRepository, ILogger<AuthService> logger, SignInManager<User> signInManager, UserManager<User> userManager, SocialMediaDataContext context)
        {
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _userRepository = userRepository;
        }
        public async Task<JWTResponseViewModel> Login(LoginViewModel loginViewModel)
        {
            var jwtResponse = new JWTResponseViewModel();
            var user = await _userManager.FindByNameAsync(loginViewModel.Email);
            if (user != null)
            {
                jwtResponse.UserId = user.Id;
                var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    var token = await GenerateJSONWebToken(user);
                    jwtResponse.Token = token;
                    jwtResponse.IsSuccess = true;
                    jwtResponse.ErrorMessage = new List<string> { "" };
                    return jwtResponse;
                }
                if (!result.Succeeded)
                {
                    jwtResponse.IsSuccess = false;
                    jwtResponse.ErrorMessage = new List<string> { "The password you entered is incorrect" };
                    return jwtResponse;
                }
                if (!result.Succeeded)
                {
                    return jwtResponse;
                }
            }
            jwtResponse.ErrorMessage = new List<string> { "The user doesnot exist" };
            return jwtResponse;
        }

        public async Task<JWTResponseViewModel> Register(SignupViewModel signupViewModel)
        {
            try
            {
                var user = new User()
                {
                    FirstName = signupViewModel.FirstName,
                    LastName = signupViewModel.LastName,
                    Email = signupViewModel.Email,
                    PhoneNumber = signupViewModel.PhoneNumber,
                    Country = signupViewModel.Country,
                    City = signupViewModel.City,
                    UserName = signupViewModel.Email,
                    DateOfBirth = signupViewModel.DateOfBirth,
                    Gender = signupViewModel.Gender.ToString(),
                };
                var result = await _userManager.CreateAsync(user, signupViewModel.Password);
                var responseModel = new JWTResponseViewModel()
                {
                    UserId = user.Id,
                };
                if(result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    responseModel.IsSuccess = true;
                    responseModel.ErrorMessage = new List<string> { "" };
                }
                else
                {
                    responseModel.IsSuccess = false;
                    responseModel.ErrorMessage = result.Errors.Select(x => x.Description).ToList();
                }
                return responseModel;
            }
            catch 
            {
                return null;
            }
            
        }

        public async Task<string> GenerateJSONWebToken(User userInfo)
        {
            var roles = await _userManager.GetRolesAsync(userInfo);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, userInfo.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.UserName),
                new Claim(JwtRegisteredClaimNames.Name,userInfo.FirstName + " " + userInfo.LastName),
                new Claim(JwtRegisteredClaimNames.AuthTime, DateAndTime.Now.ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }


            var identity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              identity.Claims,
              expires: DateTime.Now.AddDays(1),
              signingCredentials: credentials);


            var result = new JwtSecurityTokenHandler().WriteToken(token);
            return result;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<JWTResponseViewModel> ChangePassword(ChangePasswordViewModel changePasswordViewModel, string email)
        {
            var result = new JWTResponseViewModel();
            var user = await _userManager.FindByEmailAsync(email);
            
            var isUserPasswordChange = await _userManager.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.ConfirmPassword);
            if (isUserPasswordChange.Succeeded)
            {
                result.IsSuccess = true;
                result.UserId = user.Id;
                result.ErrorMessage = new List<string> { "" };
                result.Token = "";
            }
            else
            {
                result.IsSuccess = false;
                result.UserId = user.Id;
                result.ErrorMessage = isUserPasswordChange.Errors.Select(x => x.Description).ToList();
                result.Token = "";
            }
            
            return result;
        }

        public async Task<User> GetUserById(string id)
        {
            var user = await _userRepository.GetById(id);
            if(user == null)
            {
                return null;
            }
            else
            {
                return user;
            }
        }

        public async Task<string> RefreshToken(string userId)
        {
            var user = await GetUserById(userId);
            return await GenerateJSONWebToken(user);
        }
    }
}
