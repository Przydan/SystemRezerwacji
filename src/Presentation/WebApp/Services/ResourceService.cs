using System.Net.Http.Json;
using Application.Interfaces.Persistence;
using Shared.DTOs.Resource;

namespace WebApp.Services;

public class ResourceService : IResourceService
{
    private readonly HttpClient _httpClient;

    public ResourceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResourceDto?> GetResourceByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ResourceDto>($"api/resource/{id}");
        }
        catch (HttpRequestException) // Obsługuje błędy sieciowe lub statusy 404/500
        {
            return null;
        }
    }

    public async Task<List<ResourceDto>> GetAllResourcesAsync()
    {
        var resources = await _httpClient.GetFromJsonAsync<List<ResourceDto>>("api/resource");
        return resources ?? new List<ResourceDto>();
    }

    public async Task<ResourceDto> CreateResourceAsync(CreateResourceRequestDto createDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/resource", createDto);
        response.EnsureSuccessStatusCode(); // Rzuci wyjątek, jeśli odpowiedź nie jest sukcesem
        
        var createdResource = await response.Content.ReadFromJsonAsync<ResourceDto>();
        if (createdResource is null)
        {
            throw new ApplicationException("API did not return the created resource.");
        }
        return createdResource;
    }

    public async Task<bool> UpdateResourceAsync(Guid id, UpdateResourceRequestDto updateDto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/resource/{id}", updateDto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteResourceAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/resource/{id}");
        return response.IsSuccessStatusCode;
    }
}