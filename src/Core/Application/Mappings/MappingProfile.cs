using AutoMapper;
using Domain.Entities;
using Shared.DTOs.Resource;


namespace Application.Mappings
{
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapowanie dla ResourceType
        CreateMap<ResourceType, ResourceTypeDto>();
        CreateMap<CreateResourceTypeRequestDto, ResourceType>(); // Załóżmy, że masz takie DTO
        CreateMap<UpdateResourceTypeRequestDto, ResourceType>(); // Załóżmy, że masz takie DTO

        // Mapowanie dla Resource
        CreateMap<Resource, ResourceDto>()
            .ForMember(dest => dest.ResourceTypeName, opt => opt.MapFrom(src => src.ResourceType != null ? src.ResourceType.Name : string.Empty));
        // Powyższa linia mapuje Resource.ResourceType.Name na ResourceDto.ResourceTypeName.
        // Sprawdza null, aby uniknąć NullReferenceException, jeśli ResourceType nie został załadowany.

        CreateMap<CreateResourceRequestDto, Resource>();
        CreateMap<UpdateResourceRequestDto, Resource>();

        // TODO: Dodaj tutaj mapowania dla innych encji i DTOs, gdy będą potrzebne
        // np. dla Booking, User, Feature
        // CreateMap<Booking, BookingDto>();
        // CreateMap<CreateBookingRequestDto, Booking>();
    }
}

// Przykład DTO dla ResourceType, jeśli jeszcze nie istnieje w SharedKernel
// Należy go przenieść/utworzyć w SystemRezerwacji.SharedKernel.DTOs/ResourceType/
}