using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Data
{
    public class Bruker
    {
        [Key]
        public int BrukerNr { get; set; }
        [StringLength(50)]
        public string Fornavn { get; set; }
        [StringLength(40)]
        public string Etternavn { get; set; }
        [StringLength(50)]
        public string Adresse { get; set; }

        [StringLength(4)]
        public string PostNr { get; set; }
        public PostSted PostSted { get; set; }

        public ICollection<Innmelding> Innmeldinger { get; set; }
    }
}
