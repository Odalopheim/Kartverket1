using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Data
{
    public class Kategori
    {
        [Key]
        public int KatNr { get; set; }
        public string KatNavn { get; set; }

        public ICollection<GeoChange> GeoChanges { get; set; }
    }
}
