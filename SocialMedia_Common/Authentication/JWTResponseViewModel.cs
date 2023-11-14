using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Common.Authentication
{
    public class JWTResponseViewModel
    {
        public string Token { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public List<string>? ErrorMessage { get; set; }
        public string UserId { get; set; }
    }
}
