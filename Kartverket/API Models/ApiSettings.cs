using Microsoft.AspNetCore.Mvc;

namespace Kartverket.API_Models
{
    public class ApiSettings
    {
        public string KommuneInfoApiBaseUrl { get; set; }

        public string StedsnavnApiBaseUrl { get; set; }
    }
}