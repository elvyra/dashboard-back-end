using AutoMapper;
using Dashboard.Api.Models;
using Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.Profiles
{
    public class PortalQueryPostProfile : Profile
    {
        public PortalQueryPostProfile()
        {
            CreateMap<Portal, PortalQueryPostModel>()
                .ForMember(dest => dest.CallParameters, opt => opt.MapFrom(src => src.Parameters)); 
            CreateMap<PortalQueryPostModel, Portal>()
                .ForMember(dest => dest.Parameters, opt => opt.MapFrom(src => src.CallParameters));
        }
    }
}
