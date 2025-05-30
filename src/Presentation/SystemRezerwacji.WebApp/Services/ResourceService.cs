// src/Presentation/SystemRezerwacji.WebApp/Services/ResourceService.cs
using System.Net.Http;
using System.Net.Http.Json;
using SystemRezerwacji.WebApp.Models;

namespace SystemRezerwacji.WebApp.Services
{
    public interface IResourceService
    {
        Task<List<ResourceDto>> GetResourcesAsync();
    }

    public class ResourceService : IResourceService
    {
        private readonly HttpClient _http;
        public ResourceService(HttpClient http) => _http = http;

        public async Task<List<ResourceDto>> GetResourcesAsync()
        {
            // GetFromJsonAsync może zwrócić null, dlatego harujący fallback
            var list = await _http.GetFromJsonAsync<List<ResourceDto>>("api/resources");
            return list ?? new List<ResourceDto>();
        }
    }
}