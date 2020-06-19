using AutoMapper;
using Dashboard.Api.Models;
using Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.Profiles
{
    public class QueryPortalProfile : Profile
    {

        public QueryPortalProfile()
        {
            CreateMap<Portal, QueryPortalViewModel>()
                  .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.PortalResponses.FirstOrDefault().Status ))
                  .ForMember(dest => dest.ResponseTime, opt => opt.MapFrom(src => src.PortalResponses.FirstOrDefault().ResponseTime ))
                  .ForMember(dest => dest.LastFailure, opt => opt.MapFrom(src => src.PortalResponses.FirstOrDefault().RequestDateTime ));         
        }
    }
}
