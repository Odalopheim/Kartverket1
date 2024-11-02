using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Data
{
    public class Ansatt
    {
        [Key]
        public int AnsattId { get; set; }
        public DateTime FødselsDato { get; set; }

        public int BrukerId { get; set; }
        public BrukerID Bruker { get; set; }
    }
}
