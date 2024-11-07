using Kartverket.API_Models;

namespace Kartverket.Services
{
    // Interface for å hente stedsnavn asynkront
    public interface IStedsnavnService
    {
        // Asynkron metode som tar et søkebegrep og returnerer et StedsnavnResponse
        Task<StedsnavnResponse> GetStedsnavnAsync(string search);
    }
}