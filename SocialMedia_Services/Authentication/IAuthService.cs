using SocialMedia_Common.Authentication;
using SocialMedia_Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Services.Authentication
{
    public interface IAuthService
    {
        Task<JWTResponseViewModel> Login(LoginViewModel loginViewModel);
        Task<JWTResponseViewModel> Register (SignupViewModel signupViewModel);
        Task Logout();
        Task<string> GenerateJSONWebToken(User userInfo);
        Task<JWTResponseViewModel> ChangePassword(ChangePasswordViewModel changePasswordViewModel, string email);
        //Task<bool> SendEmail(List<string> to, List<string> cc, List<string> bcc, string subject, string body, string? pdfContent, string? pdfName);
        Task<User> GetUserById(string id);

        Task<string> RefreshToken(string userId);
    }
}
