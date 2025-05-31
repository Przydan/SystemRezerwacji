// Application/Services/IResourceTypeService.cs

using Shared.DTOs.Resource;

namespace Application.Services;

public interface IResourceTypeService
{
    Task<List<ResourceTypeDto>> GetAllResourceTypesAsync();
}