using System.Net.Http.Json;
using Shared.DTOs.Resource;

public class ResourceClientService
{
    private readonly HttpClient _http;
    public ResourceClientService(HttpClient http) => _http = http;

    public Task<List<ResourceDto>> GetResourcesAsync() =>
        _http.GetFromJsonAsync<List<ResourceDto>>("/api/resources");

    public Task<List<ResourceTypeDto>> GetResourceTypesAsync() =>
        _http.GetFromJsonAsync<List<ResourceTypeDto>>("/api/resourcetypes");

    public Task<bool> CreateResourceAsync(ResourceDto dto) =>
        _http.PostAsJsonAsync("/api/resources", dto)
            .ContinueWith(t => t.Result.IsSuccessStatusCode);

    public Task<bool> UpdateResourceAsync(ResourceDto dto) =>
        _http.PutAsJsonAsync($"/api/resources/{dto.Id}", dto)
            .ContinueWith(t => t.Result.IsSuccessStatusCode);

    public Task<bool> DeleteResourceAsync(Guid id) =>
        _http.DeleteAsync($"/api/resources/{id}")
            .ContinueWith(t => t.Result.IsSuccessStatusCode);
}