using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Kartverket.Data
{

    public class GeoChange
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? GeoJson { get; set; }
        [Required]
        public string? Description { get; set; }

        [BindNever]
        public string UserId { get; set; }
        public GeoChangeStatus Status { get; set; }
        [Required] public DateTime CreatedDate { get; set; } = DateTime.Now; // Automatisk dato ved opprettelse
        public GeoChangeCategory Category {  get; set; } 
    } 
    public enum GeoChangeStatus 
    { 
        [Display(Name = "Sendt inn")] SendtInn, 
        [Display(Name = "Godtatt")] Godtatt, 
        [Display(Name = "Behandles")] Behandles, 
        [Display(Name = "Fullført")] Fullført, 
        [Display(Name = "Avvist")] Avvist

    }

    public enum GeoChangeCategory
    {
        [Display(Name = "Stedsnavn")]
        Stedsnavn,
        [Display(Name = "Veinavn")]
        Veinavn,
        [Display(Name = "Tursti")]
        Tursti,
        [Display(Name = "Bilvei")]
        Bilvei,
        [Display(Name = "Annet")]
        Annet
    }



}
