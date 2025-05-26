using SystemRezerwacji.WebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SystemRezerwacji.WebApp.Services;

public interface IResourceTypeService
{
    Task<List<ResourceTypeDto>?> GetResourceTypesAsync();
}