using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Data.Entity
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public override string Email { get; set; } = string.Empty;
        public override string PhoneNumber { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Avatar { get; set; } = string.Empty;
        public string Gender { get; set;} = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public bool IsAccountDeleted { get; set; } = false;
        public DateTime? DateOfBirth { get; set; }

    }
}
