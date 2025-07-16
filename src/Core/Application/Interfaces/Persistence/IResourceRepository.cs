using Domain.Entities;

namespace Application.Interfaces.Persistence;

public interface IResourceRepository
{
    Task<Resource?> GetByIdAsync(Guid id);
    Task<List<Resource>> GetAllAsync();
    Task AddAsync(Resource resource);
    Task UpdateAsync(Resource resource);
    Task DeleteAsync(Guid id);
}