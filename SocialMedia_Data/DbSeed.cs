using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialMedia_Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Data
{
    public class DbSeed
    {
        private readonly SocialMediaDataContext _socialMediaDataContext;
        private readonly UserManager<User> _userManager;
        public DbSeed(SocialMediaDataContext socialMediaDataContext, UserManager<User> userManager)
        {
            _socialMediaDataContext = socialMediaDataContext;
            _userManager = userManager;
        }
        public async Task SeedAsync()
        {
            _socialMediaDataContext.Database.EnsureCreated();
            await SeedRoles();
            await SeedUser();
        }
        public async Task SeedRoles()
        {
            var roles = await _socialMediaDataContext.Roles.CountAsync();
            if (roles == 0)
            {
                _socialMediaDataContext.Roles.Add(new IdentityRole { Name = "User", NormalizedName = "User" });
                _socialMediaDataContext.Roles.Add(new IdentityRole { Name = "Admin", NormalizedName = "Admin" });
                await _socialMediaDataContext.SaveChangesAsync();
            }
        }
        public async Task SeedUser()
        {
            var user = await _userManager.FindByEmailAsync("admin@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    Email = "admin@gmail.com",
                    UserName = "admin@gmail.com",
                    FirstName = "Admin",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    Gender = "Male",
                };

                var result = await _userManager.CreateAsync(user, "Sizzling@56");
                if (result == IdentityResult.Success)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
