// SystemRezerwacji.Application/Features/ResourceType/Queries/GetAllResourceTypes/GetAllResourceTypesQuery.cs
using MediatR;
using SystemRezerwacji.Application.DTOs.ResourceType;
using System.Collections.Generic;

namespace SystemRezerwacji.Application.Features.ResourceType.Queries.GetAllResourceTypes;

public class GetAllResourceTypesQuery : IRequest<List<ResourceTypeDto>>
{
}