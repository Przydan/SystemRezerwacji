using System.Net.Http.Json;
using Application.Interfaces.Persistence;
using Shared.DTOs.Resource;

namespace WebApp.Services;

public class ResourceTypeService : IResourceTypeService
{
    private readonly HttpClient _httpClient;

    public ResourceTypeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResourceTypeDto?> GetResourceTypeByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<ResourceTypeDto>($"api/resourcetypes/{id}");
    }

    public async Task<List<ResourceTypeDto>> GetAllResourceTypesAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<ResourceTypeDto>>("api/resourcetypes");
        return result ?? new List<ResourceTypeDto>();
    }

    public async Task<ResourceTypeDto> CreateResourceTypeAsync(CreateResourceTypeRequestDto createDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/resourcetypes", createDto);
        response.EnsureSuccessStatusCode();
        
        var createdDto = await response.Content.ReadFromJsonAsync<ResourceTypeDto>();
        if (createdDto is null)
        {
            throw new ApplicationException("API did not return the created resource type.");
        }
        return createdDto;
    }

    public async Task<bool> UpdateResourceTypeAsync(Guid id, UpdateResourceTypeRequestDto updateDto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/resourcetypes/{id}", updateDto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteResourceTypeAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/resourcetypes/{id}");
        return response.IsSuccessStatusCode;
    }
}