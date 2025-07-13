using Shared.DTOs.Resource;

namespace WebApp.Services;

public interface IResourceService
{
    Task<List<ResourceDto>> GetResourcesAsync();
    Task CreateResourceAsync(CreateResourceRequestDto model); // Zmień typ parametru
    Task UpdateResourceAsync(Guid id, UpdateResourceRequestDto model); // Dodaj ID jako parametr
}