using AutoMapper;
using Dashboard.Api.Models;
using Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.Profiles
{
    public class PortalResponseProfile : Profile
    {
        public PortalResponseProfile()
        {
            CreateMap<PortalResponse, PortalResponseViewModel>();
        }
    }
}
