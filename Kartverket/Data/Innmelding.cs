using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Data
{
    public class Innmelding
    {
        [Key]
        public int InnmeldingId { get; set; }
        public DateTime Dato { get; set; }
        [StringLength(300)]
        public string Beskrivelse { get; set; }

        public int BrukerId { get; set; }
        public Bruker Bruker { get; set; }

        public int KatNr { get; set; }
        public Kategori Kategori { get; set; }
    }
}
