﻿namespace Kartverket.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    public class RoleInitializer
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Roller som skal opprettes (add Admin role here)
            var roles = new[] { "Saksbehandler", "Bruker", "Admin" };

            // Opprett roller hvis de ikke finnes
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Opprett en standard saksbehandler-bruker
            string saksbehandlerEmail = "saksbehandler@Kartverket.no";
            string saksbehandlerPassword = "Saksbehandler123!";

            var saksbehandler = await userManager.FindByEmailAsync(saksbehandlerEmail);
            if (saksbehandler == null)
            {
                saksbehandler = new IdentityUser
                {
                    UserName = saksbehandlerEmail,
                    Email = saksbehandlerEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(saksbehandler, saksbehandlerPassword);
            }

            if (!await userManager.IsInRoleAsync(saksbehandler, "Saksbehandler"))
            {
                await userManager.AddToRoleAsync(saksbehandler, "Saksbehandler");
            }

            // Opprett en standard bruker
            string brukerEmail = "bruker@eksempel.no";
            string brukerPassword = "Bruker123!";

            var bruker = await userManager.FindByEmailAsync(brukerEmail);
            if (bruker == null)
            {
                bruker = new IdentityUser
                {
                    UserName = brukerEmail,
                    Email = brukerEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(bruker, brukerPassword);
            }

            if (!await userManager.IsInRoleAsync(bruker, "Bruker"))
            {
                await userManager.AddToRoleAsync(bruker, "Bruker");
            }

            // Opprett en standard admin-bruker
            string adminEmail = "admin@Kartverket.no";
            string adminPassword = "Admin123!";

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, adminPassword);
            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
