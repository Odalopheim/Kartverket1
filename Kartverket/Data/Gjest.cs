using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Data
{
    public class Gjest
    {
        [Key]
        public int GjestId { get; set; }

    }
}
