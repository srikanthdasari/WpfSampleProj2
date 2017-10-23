using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfSampleProj2.Services.Entities;
using WpfSampleProj2.Services.Models;

namespace WpfSampleProj2.Services.MappingProfiles
{
    public class AuthorForCreationMapperProfile:Profile
    {
        public AuthorForCreationMapperProfile()
        {
            CreateMap<AuthorForCreationDto, Author>();
        }
    }
}
