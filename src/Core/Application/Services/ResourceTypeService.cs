using Application.Interfaces.Persistence;
using Shared.DTOs.Resource;


namespace Application.Services;

public class ResourceTypeService : IResourceTypeService
{
    private readonly IResourceTypeRepository _resourceTypeRepository;
    // private readonly IMapper _mapper;

    public ResourceTypeService(IResourceTypeRepository resourceTypeRepository /*, IMapper mapper*/)
    {
        _resourceTypeRepository = resourceTypeRepository;
        // _mapper = mapper;
    }

    public async Task<List<ResourceTypeDto>> GetAllResourceTypesAsync()
    {
        var resourceTypes = await _resourceTypeRepository.GetAllAsync();
        // Ręczne mapowanie lub AutoMapper
        var resourceTypeDtos = new List<ResourceTypeDto>();
        foreach (var rt in resourceTypes)
        {
            resourceTypeDtos.Add(new ResourceTypeDto
            {
                Id = rt.Id,
                Name = rt.Name,
                Description = rt.Description,
                IconCssClass = rt.IconCssClass
            });
        }
        return resourceTypeDtos;
    }
}