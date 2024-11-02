using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Data
{
    public class Innmelder
    {
        [Key]
        public int InnmelderId { get; set; }

        public int BrukerId { get; set; }
        public Bruker Bruker { get; set; }
    }
}
