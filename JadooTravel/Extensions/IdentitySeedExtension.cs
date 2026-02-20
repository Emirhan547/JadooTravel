using JadooTravel.Entity.Entities;
using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppRole = JadooTravel.Entity.Entities.AppRole;

namespace JadooTravel.UI.Extensions
{
    public static class IdentitySeedExtension
    {
        public static async Task SeedAdminAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            const string adminRole = "Admin";
            const string adminEmail = "admin@jadoo.com";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new AppRole
                {
                    Name = adminRole
                });
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser != null) return;

            adminUser = new AppUser
            {
                UserName = "admin",
                Email = adminEmail,
                FullName = "Admin User",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(adminUser, "Admin123");
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}
