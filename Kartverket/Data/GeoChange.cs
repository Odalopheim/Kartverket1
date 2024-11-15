using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Kartverket.Data
{

    public class GeoChange
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? GeoJson { get; set; }
        [Required]
        public string? Description { get; set; }
        public string? Category { get; set; }

        [BindNever]
        public string UserId { get; set; }

        //public List<Vedlegg> Vedlegg { get; set; } = new List<Vedlegg>();
    }

    /*public class Vedlegg
    {
        public int Id { get; set; }
        public string FilNavn { get; set; }
        public byte[] FilData { get; set; }
    }*/

}
