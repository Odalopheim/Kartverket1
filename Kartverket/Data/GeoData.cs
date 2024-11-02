using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Data
{
    public class GeoData
    {
        [Key]
        public int GeoId { get; set; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }

    }
}
