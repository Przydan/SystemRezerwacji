using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Persistence;
using Domain.Entities;
using Infrastructure.Persistence.DbContext;

namespace Infrastructure.Persistence.Repositories;

public class ResourceTypeRepository : IResourceTypeRepository
{
    private readonly SystemRezerwacjiDbContext _dbContext;

    public ResourceTypeRepository(SystemRezerwacjiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ResourceType?> GetByIdAsync(Guid id)
    {
        return await _dbContext.ResourceTypes.FindAsync(id);
    }

    public async Task<IReadOnlyList<ResourceType>> GetAllAsync()
    {
        return await _dbContext.ResourceTypes.AsNoTracking().ToListAsync();
    }

    public async Task<ResourceType> AddAsync(ResourceType resourceType)
    {
        await _dbContext.ResourceTypes.AddAsync(resourceType);
        await _dbContext.SaveChangesAsync();
        return resourceType;
    }

    public async Task UpdateAsync(ResourceType resourceType)
    {
        var existingResourceType = await _dbContext.ResourceTypes.FindAsync(resourceType.Id);
        
        if (existingResourceType == null)
        {
            return;
        }
        
        existingResourceType.Name = resourceType.Name;
        existingResourceType.Description = resourceType.Description;
        existingResourceType.IconCssClass = resourceType.IconCssClass;
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(ResourceType resourceType)
    {
        _dbContext.ResourceTypes.Remove(resourceType);
        await _dbContext.SaveChangesAsync();
    }
}