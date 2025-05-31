// Infrastructure/Persistence/Repositories/ResourceTypeRepository.cs

using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Persistence;
using Domain.Entities;
// Upewnij się, że jest poprawna ścieżka
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        _dbContext.Entry(resourceType).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(ResourceType resourceType)
    {
        _dbContext.ResourceTypes.Remove(resourceType);
        await _dbContext.SaveChangesAsync();
    }
}