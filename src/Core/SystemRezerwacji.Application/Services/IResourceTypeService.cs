// SystemRezerwacji.Application/Services/IResourceTypeService.cs
using SystemRezerwacji.Application.DTOs.ResourceType;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SystemRezerwacji.Application.Services;

public interface IResourceTypeService
{
    Task<List<ResourceTypeDto>> GetAllResourceTypesAsync();
}