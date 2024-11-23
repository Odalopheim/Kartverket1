using Microsoft.AspNetCore.Identity;
namespace Kartverket.Data
{

    public class CustomUser : IdentityUser
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PostNumber { get; set; }
    }
}
