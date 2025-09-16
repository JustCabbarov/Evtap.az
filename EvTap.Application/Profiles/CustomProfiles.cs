using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EvTap.Contracts.DTOs;
using EvTap.Domain.Entities;

namespace EvTap.Application.Profiles
{
    public class CustomProfiles : Profile
    {
        public CustomProfiles()
        {
            CreateMap<City, CityDTO>().ReverseMap();
            CreateMap<District, DistrictDTO>().ReverseMap();
            CreateMap<MetroStation, MetroStationDTO>().ReverseMap();
            CreateMap<AgencyDTO, Agency>().ReverseMap();
            CreateMap<CategoryDTO, Category>().ReverseMap();
           
         
        
        }
    }
}
