using AutoMapper;
using Dashboard.Api.Models;
using Dashboard.Models;
using Dashboard.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Dashboard.Api.Profiles
{
    public class PortalFullProfile : Profile
    {
        public PortalFullProfile()
        {
            CreateMap<Portal, PortalFullViewModel>()
               .ForMember(dest => dest.CallParameters, opt => opt.MapFrom(src => src.Parameters))
               .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == PortalStatus.Active));

            CreateMap<PortalFullViewModel, Portal>()
               .ForMember(dest => dest.Parameters, opt => opt.MapFrom(src => src.CallParameters))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? PortalStatus.Active : PortalStatus.NotActive));         
        }
    }
}
