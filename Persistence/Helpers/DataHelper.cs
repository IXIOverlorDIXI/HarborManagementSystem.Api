using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Consts;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers
{
    public static class DataHelper
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            await roleManager.CreateAsync(new IdentityRole(Roles.User));
        }
        
        public static async Task SeedAdminsAsync(UserManager<AppUser> userManager)
        {
            var admin = new AppUser
            {
                UserName = "admin",
                DisplayName = "Admin",
                Email = "admin@gmail.com"
            };

            await userManager.CreateAsync(admin, "Admin2023");
            await userManager.AddToRoleAsync(admin, Roles.Admin);
        }

        public static IEnumerable<Subscription> SeedSubscriptions()
        {
            var subscriptions = new List<Subscription>
            {
                new Subscription
                {
                    Description = "Free subscription for common user.",
                    DisplayName = "Default subscription",
                    Id = Guid.NewGuid(),
                    MaxHarborAmount = 0,
                    Price = 0.0,
                    TaxOnBooking = 0,
                    TaxOnServices = 0
                }
            };

            return subscriptions;
        }
    }
}