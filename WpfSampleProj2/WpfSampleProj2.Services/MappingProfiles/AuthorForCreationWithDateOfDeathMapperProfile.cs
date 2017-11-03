﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfSampleProj2.Services.Entities;
using WpfSampleProj2.Services.Models;

namespace WpfSampleProj2.Services.MappingProfiles
{
    public class AuthorForCreationWithDateOfDeathMapperProfile : Profile
    {
        public AuthorForCreationWithDateOfDeathMapperProfile()
        {
            CreateMap<AuthorForCreationWithDateOfDeathDto, Author>().ReverseMap().IgnoreAllPropertiesWithAnInaccessibleSetter();
        }
    }
}
