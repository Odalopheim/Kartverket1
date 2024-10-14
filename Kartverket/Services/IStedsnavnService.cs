using Kartverket.API_Models;

namespace Kartverket.Services
{
    public interface IStedsnavnService
    {
        Task<StedsnavnResponse> GetStedsnavnAsync(string search);
    }
}
