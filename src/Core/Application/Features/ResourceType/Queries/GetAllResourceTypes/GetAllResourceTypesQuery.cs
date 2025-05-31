// Application/Features/ResourceType/Queries/GetAllResourceTypes/GetAllResourceTypesQuery.cs

using MediatR;
using Shared.DTOs.Resource;

namespace Application.Features.ResourceType.Queries.GetAllResourceTypes;

public class GetAllResourceTypesQuery : IRequest<List<ResourceTypeDto>>
{
}