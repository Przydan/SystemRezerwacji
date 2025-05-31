using Shared.DTOs.Resource;

namespace WebApp.Services
{
    public interface IResourceServiceTemp
    {
        Task<List<ResourceDto>> GetResourcesAsync();
        Task CreateResourceAsync(ResourceDto dto);
        Task UpdateResourceAsync(ResourceDto dto);
        Task DeleteResourceAsync(Guid id);
    }
}