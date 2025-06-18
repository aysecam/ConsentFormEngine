using AutoMapper;
using ConsentFormEngine.Business.DTOs;
using ConsentFormEngine.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsentFormEngine.Business.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, CreateUserRequestDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
            CreateMap<User, ForgotOrCreateUserRequestDto>()
                 .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                 .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname))

                 .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }

}
