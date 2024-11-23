using Kartverket.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kartverket.Migrations
{
    public partial class AddRolesToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed roles directly in the database
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>())),
                null, null, null, null
            );

            // Opprett rollen "Saksbehandler" hvis den ikke eksisterer
            if (!roleManager.RoleExistsAsync("Saksbehandler").Result)
            {
                roleManager.CreateAsync(new IdentityRole("Saksbehandler")).Wait();
            }

            // Opprett rollen "Bruker" hvis den ikke eksisterer
            if (!roleManager.RoleExistsAsync("Bruker").Result)
            {
                roleManager.CreateAsync(new IdentityRole("Bruker")).Wait();
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Fjern rollene hvis du ruller tilbake migrasjonen
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>())),
                null, null, null, null
            );

            if (roleManager.RoleExistsAsync("Saksbehandler").Result)
            {
                var role = roleManager.FindByNameAsync("Saksbehandler").Result;
                roleManager.DeleteAsync(role).Wait();
            }

            if (roleManager.RoleExistsAsync("Bruker").Result)
            {
                var role = roleManager.FindByNameAsync("Bruker").Result;
                roleManager.DeleteAsync(role).Wait();
            }
        }
    }
}

