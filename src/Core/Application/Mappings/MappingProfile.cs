using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Shared.DTOs.Auth;
using Shared.DTOs.Booking;
using Shared.DTOs.Resource;
using Shared.DTOs.User;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // --- Mapowania dla Zasobów (Resource) ---

            // Mapowanie z Encji Resource na DTO
            CreateMap<Resource, ResourceDto>()
                .ForMember(dest => dest.ResourceTypeName,
                    opt => opt.MapFrom(src => src.ResourceType.Name));

            // Mapowanie z DTO do tworzenia nowej Encji Resource
            CreateMap<CreateResourceRequestDto, Resource>();

            // Mapowanie z DTO do aktualizacji Encji Resource
            CreateMap<UpdateResourceRequestDto, Resource>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignorujemy ID, aby nie nadpisać go z DTO

            // --- Mapowania dla Typów Zasobów (ResourceType) ---

            // Mapowanie z Encji ResourceType na DTO
            CreateMap<ResourceType, ResourceTypeDto>();

            // Mapowanie z DTO do tworzenia nowej Encji ResourceType
            CreateMap<CreateResourceTypeRequestDto, ResourceType>();
            
            // Mapowanie z DTO do aktualizacji Encji ResourceType
            CreateMap<UpdateResourceTypeRequestDto, ResourceType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignorujemy ID, aby nie nadpisać go z DTO


            // --- Mapowania dla Rezerwacji (Booking) ---

            // Mapowanie z Encji Booking na DTO
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.ResourceName,
                    opt => opt.MapFrom(src => src.Resource.Name))
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src =>
                        !string.IsNullOrWhiteSpace(src.User.FirstName) && !string.IsNullOrWhiteSpace(src.User.LastName)
                            ? $"{src.User.FirstName} {src.User.LastName}"
                            : src.User.UserName))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));

            // Mapowanie z DTO do tworzenia nowej Encji Booking
            CreateMap<BookingRequestDto, Booking>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => BookingStatus.Confirmed)) 
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.User, opt => opt.Ignore()) 
                .ForMember(dest => dest.Resource, opt => opt.Ignore())
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

            // Mapowanie z DTO do aktualizacji Encji Booking
            CreateMap<UpdateBookingRequestDto, Booking>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));
                
            // --- Mapowania dla Użytkowników i Autoryzacji (User & Auth) ---
            CreateMap<User, UserDto>();
            
            // Mapowanie z DTO rejestracji na Encję User
            CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));
        }
    }
}