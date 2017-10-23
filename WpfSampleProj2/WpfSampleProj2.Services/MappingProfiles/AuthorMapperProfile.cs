using AutoMapper;
using WpfSampleProj2.Lib.Extensions;
using WpfSampleProj2.Services.Entities;
using WpfSampleProj2.Services.Models;

namespace WpfSampleProj2.Services.MappingProfiles
{
    public class AuthorMapperProfile:Profile
    {
        public AuthorMapperProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(d => d.Name, o => o.ResolveUsing(s => s.FirstName + ", " + s.LastName))
                .ForMember(d => d.Age, o => o.ResolveUsing(s => s.DateOfBirth.GetCurrentAge()));
                
        }
    }
}
 