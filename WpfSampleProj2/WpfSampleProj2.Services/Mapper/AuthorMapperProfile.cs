using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfSampleProj2.Services.Entities;
using WpfSampleProj2.Services.Models;

namespace WpfSampleProj2.Services.Mapper
{
    public class AuthorMapperProfile:Profile
    {
        public override string ProfileName
        {
            get { return "AuthorMappings"; }
        }

        public AuthorMapperProfile()
        {
            CreateMap<Author, AuthorDto>();
        }
    }
}
