// Application/Services/IResourceTypeService.cs

using Shared.DTOs.Resource;

namespace Application.Interfaces.Persistence
{
    public interface IResourceTypeService
    {
        Task<List<ResourceTypeDto>> GetAllResourceTypesAsync();
        Task<ResourceTypeDto?> GetResourceTypeByIdAsync(Guid id); // Zwracamy nullable DTO
        Task<ResourceTypeDto> CreateResourceTypeAsync(CreateResourceTypeRequestDto createDto);
        Task<bool> UpdateResourceTypeAsync(Guid id, UpdateResourceTypeRequestDto updateDto); // Zwracamy bool dla sukcesu
        Task<bool> DeleteResourceTypeAsync(Guid id); // Zwracamy bool dla sukcesu
    }
}
