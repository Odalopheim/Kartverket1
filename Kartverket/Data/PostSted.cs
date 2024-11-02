using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Data
{
    public class PostSted 
    {
        [Key]
        [StringLength(4)]
        public string PostNr { get; set; }
        [StringLength(30)]
        public string Sted { get; set; }

        public ICollection<Bruker> Brukere { get; set; }
    }
}
