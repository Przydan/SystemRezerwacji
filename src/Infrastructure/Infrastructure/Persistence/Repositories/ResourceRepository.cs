using Application.Interfaces.Persistence;
using Domain.Entities;
using Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly SystemRezerwacjiDbContext _dbContext;

        public ResourceRepository(SystemRezerwacjiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Resource?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Resources
                .Include(r => r.ResourceType) // Dołączamy powiązany typ zasobu
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Resource>> GetAllAsync()
        {
            return await _dbContext.Resources
                .Include(r => r.ResourceType) // Dołączamy typy, aby wyświetlić ich nazwy
                .ToListAsync();
        }

        public async Task AddAsync(Resource resource)
        {
            await _dbContext.Resources.AddAsync(resource);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Resource resource)
        {
            var existingResource = await _dbContext.Resources.FindAsync(resource.Id);
            if (existingResource != null)
            {
                existingResource.Name = resource.Name;
                existingResource.Description = resource.Description;
                existingResource.ResourceTypeId = resource.ResourceTypeId;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var resourceToDelete = await _dbContext.Resources.FindAsync(id);
            if (resourceToDelete != null)
            {
                _dbContext.Resources.Remove(resourceToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}