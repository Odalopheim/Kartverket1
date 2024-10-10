using Microsoft.AspNetCore.Mvc;

namespace Kartverket.Models;

public class AreaChange
{
    public string Id { get; set; }
    public string GeoJson { get; set; }
    public string Description { get; set; }
    public string Category { get; set; } 
}
