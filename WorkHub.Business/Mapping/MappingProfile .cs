using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.Models.DTOs;
using WorkHub.Models.Models;

namespace WorkHub.Business.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RecruitmentInfo, RecruitmentOverviewInfoDto>();
            CreateMap<RecruitmentOverviewInfoDto, RecruitmentInfo>();

        }
    }
}
