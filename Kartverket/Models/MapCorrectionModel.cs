using System.ComponentModel.DataAnnotations;

namespace Kartverk.Mvc.Models
{
    public class MapCorrectionModel
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public string X { get; set; }
        [Required]
        public string Y { get; set; }
    }
}
