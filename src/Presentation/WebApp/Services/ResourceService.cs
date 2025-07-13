using System.Net.Http.Json;
using Shared.DTOs.Resource;
using WebApp.Services;

public class ResourceService : IResourceService
{
    private readonly HttpClient _http;
    public ResourceService(HttpClient http) => _http = http;

    public async Task<List<ResourceDto>> GetResourcesAsync()
    {
        var list = await _http.GetFromJsonAsync<List<ResourceDto>>("api/resources");
        return list ?? new List<ResourceDto>();
    }

    // Uzupełniona metoda
    public async Task CreateResourceAsync(CreateResourceRequestDto model)
    {
        var response = await _http.PostAsJsonAsync("api/resources", model);
        response.EnsureSuccessStatusCode();
    }

    // Uzupełniona metoda
    public async Task UpdateResourceAsync(Guid id, UpdateResourceRequestDto model)
    {
        var response = await _http.PutAsJsonAsync($"api/resources/{id}", model);
        response.EnsureSuccessStatusCode();
    }
}
