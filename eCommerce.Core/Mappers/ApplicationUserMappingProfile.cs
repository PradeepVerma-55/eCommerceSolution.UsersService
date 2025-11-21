using AutoMapper;
using eCommerce.Core.DTO;
using eCommerce.Core.Entities;

namespace eCommerce.Core.Mappers
{
    public class ApplicationUserMappingProfile: Profile
    {
        public ApplicationUserMappingProfile()
        {
            // Define your object-object mapping configurations here
            CreateMap<ApplicationUser, AuthenticationResponse>()
                .ForMember(dest=> dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.PersonName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src =>src.Gender))
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.success, opt => opt.Ignore())
                ;
        }
    }
}
