using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialMedia_DataAccess.Enum;

namespace SocialMedia_Common.Authentication
{
    public class SignupViewModel
    {
        [Required]
        [MinLength(2, ErrorMessage = "First name must contain at least 2 character"), MaxLength(15, ErrorMessage = "First name must contain at least 15 character")]
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare("Password", ErrorMessage = "Password and Confirm does not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        [Required]
        public GenderType Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }

    }


}
