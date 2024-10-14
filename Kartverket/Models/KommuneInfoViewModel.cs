using Microsoft.AspNetCore.Mvc;

namespace Kartverket.Models
{
    public class KommuneInfoViewModel 
    {
        public string? Kommunenavn { get; set; }    
        public string? Kommunenummer { get; set; } 
        public string? Fylkesnavn { get; set; }
        public bool SamiskForvaltningsomrade { get; set; }
    }
}
