using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Kartverket.Models;


namespace Kartverket.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>// DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<GeoChange> GeoChanges { get; set; }
       


       
    }
}
