﻿using Microsoft.AspNetCore.Mvc;

namespace Kartverket.Models
{
    public class StedsnavnViewModel 
    {
        public string? Skrivemåte {  get; set; }
        public string? Navneobjekttype { get; set; }
        public string? Språk {  get; set; }
        public string? Navnestatus { get; set; }
        public string? Stedstatus { get; set; }

    }
}