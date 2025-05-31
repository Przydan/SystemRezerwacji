using Shared.DTOs.Resource;

namespace WebApp.Services;

public interface IResourceTypeService
{
    Task<List<ResourceTypeDto>?> GetResourceTypesAsync();
}