using SystemRezerwacji.Domain.Entities;

namespace SystemRezerwacji.Application.Interfaces.Persistence;

public interface IResourceTypeRepository
{
    Task<ResourceType?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ResourceType>> GetAllAsync();
    Task<ResourceType> AddAsync(ResourceType resourceType);
    Task UpdateAsync(ResourceType resourceType);
    Task DeleteAsync(ResourceType resourceType); // lub Task DeleteAsync(Guid id);
}