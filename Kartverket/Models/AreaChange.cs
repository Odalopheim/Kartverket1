using System.ComponentModel.DataAnnotations;

namespace Kartverket.Models
{
    public class AreaChange
    {
        [Required(ErrorMessage = "Id is required.")]
        public string Id { get; set; }  
    }
}
