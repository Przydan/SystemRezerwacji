using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemRezerwacji.WebApp.Models;

namespace SystemRezerwacji.WebApp.Services
{
    public interface IResourceService
    {
        Task<List<ResourceDto>> GetResourcesAsync();
        Task CreateResourceAsync(ResourceDto dto);
        Task UpdateResourceAsync(ResourceDto dto);
        Task DeleteResourceAsync(Guid id);
    }
}