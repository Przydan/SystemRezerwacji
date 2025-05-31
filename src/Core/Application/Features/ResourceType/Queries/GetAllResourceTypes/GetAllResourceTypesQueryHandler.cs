using Application.Interfaces.Persistence;
using MediatR;
using Shared.DTOs.Resource;

namespace Application.Features.ResourceType.Queries.GetAllResourceTypes;

public class GetAllResourceTypesQueryHandler : IRequestHandler<GetAllResourceTypesQuery, List<ResourceTypeDto>>
{
    private readonly IResourceTypeRepository _resourceTypeRepository;
    // private readonly IMapper _mapper; // Wstrzyknij IMapper, jeśli używasz

    // Jeśli nie używasz AutoMappera na razie, możesz mapować ręcznie
    public GetAllResourceTypesQueryHandler(IResourceTypeRepository resourceTypeRepository /*, IMapper mapper*/)
    {
        _resourceTypeRepository = resourceTypeRepository;
        // _mapper = mapper;
    }

    public async Task<List<ResourceTypeDto>> Handle(GetAllResourceTypesQuery request,
        CancellationToken cancellationToken)
    {
        var resourceTypes = await _resourceTypeRepository.GetAllAsync();

        // Ręczne mapowanie jako przykład (docelowo AutoMapper)
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

        // return _mapper.Map<List<ResourceTypeDto>>(resourceTypes); // Z AutoMapperem
        return resourceTypeDtos;
    }
}