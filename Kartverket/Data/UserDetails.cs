using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Kartverket.Data
{
    public class UserDetails
    {
        [Key]
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PostNumber { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
    }
}
