using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfSampleProj2.Services.Entities;
using WpfSampleProj2.Services.Extensions;
using WpfSampleProj2.Services.Models;

namespace WpfSampleProj2.Services.MappingProfiles
{
    public class BookForUpdateMapperProfile:Profile
    {
        public BookForUpdateMapperProfile()
        {
            CreateMap<BookForUpdateDto, Book>()
                .ForMember(d=> d.AuthorId,o=>o.Ignore())
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description)).ReverseMap();

        }
    }
}
