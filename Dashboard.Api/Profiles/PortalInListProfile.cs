using AutoMapper;
using Dashboard.Api.Models;
using Dashboard.Models;
using Dashboard.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.Profiles
{
    public class PortalInListProfile : Profile
    {
        public PortalInListProfile()
        {
            CreateMap<Portal, PortalInListViewModel>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == PortalStatus.Active));
            CreateMap<PortalInListViewModel, Portal>()
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? PortalStatus.Active : PortalStatus.NotActive));
        }
    }
}
