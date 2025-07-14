using Shared.DTOs.Resource;

namespace Application.Interfaces.Persistence;

public interface IResourceService
{
    Task<ResourceDto?> GetResourceByIdAsync(Guid id);
    Task<List<ResourceDto>> GetAllResourcesAsync();
    Task<ResourceDto> CreateResourceAsync(CreateResourceRequestDto createDto);
    Task<bool> UpdateResourceAsync(Guid id, UpdateResourceRequestDto updateDto);
    Task<bool> DeleteResourceAsync(Guid id);
}