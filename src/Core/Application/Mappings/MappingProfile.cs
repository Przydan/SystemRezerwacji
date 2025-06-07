using AutoMapper;
using Domain.Entities;
using Shared.DTOs.Booking;
using Shared.DTOs.Resource;


namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<ResourceType, ResourceTypeDto>();
            CreateMap<CreateResourceTypeRequestDto, ResourceType>();
            CreateMap<UpdateResourceTypeRequestDto, ResourceType>(); 


            CreateMap<Resource, ResourceDto>()
                .ForMember(dest => dest.ResourceTypeName,
                    opt => opt.MapFrom(src => src.ResourceType != null ? src.ResourceType.Name : string.Empty));

            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.ResourceName,
                    opt => opt.MapFrom(src => src.Resource != null ? src.Resource.Name : string.Empty))
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src =>
                        (src.User.FirstName + " " + src.User.LastName).Trim()))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));


            CreateMap<CreateResourceRequestDto, Resource>();
            CreateMap<UpdateResourceRequestDto, Resource>();
            CreateMap<Booking, BookingDto>();
            CreateMap<ResourceType, ResourceTypeDto>();
            // CreateMap<CreateBookingRequestDto, Booking>();
        }
    }

}