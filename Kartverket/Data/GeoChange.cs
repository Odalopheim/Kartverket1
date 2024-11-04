using Microsoft.AspNetCore.Mvc;

namespace Kartverket.Data
{

    public class GeoChange
    {
        public int Id { get; set; }
        public string? GeoJson { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public List<Vedlegg> Vedlegg { get; set; } = new List<Vedlegg>();
    }

    public class Vedlegg
    {
        public int Id { get; set; }
        public string FilNavn { get; set; }
        public byte[] FilData { get; set; }
    }

}
