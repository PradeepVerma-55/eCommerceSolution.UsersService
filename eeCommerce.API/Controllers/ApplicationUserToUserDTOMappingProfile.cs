using AutoMapper;
using eCommerce.Core.DTO;
using eCommerce.Core.Entities;

namespace eCommerce.API.Controllers
{
    public class ApplicationUserToUserDTOMappingProfile : Profile
    {
        public ApplicationUserToUserDTOMappingProfile()
        {
            CreateMap<ApplicationUser, UserDTO>()
              .ForMember(des => des.UserId, opt => opt.MapFrom(src => src.UserId))
              .ForMember(des => des.Email, opt => opt.MapFrom(src => src.Email))
              .ForMember(des => des.PersonName, opt => opt.MapFrom(src => src.PersonName))
              .ForMember(des => des.Gender, opt => opt.MapFrom(src => src.Gender));
        }
    }
}
