using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Data
{
    public class BrukerID
    {
        [Key]
        public int BrukerId { get; set; }
        [StringLength(321)]
        public string Epost { get; set; }
        [StringLength(321)]
        public string Brukernavn { get; set; }
        [StringLength(50)]
        public string Passord { get; set; }
    }
}
