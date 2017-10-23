using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WpfSampleProj2.Services.Extensions
{
    public static class MapperExtension
    {
        public static T Map<T>(this object source)
        {
            return AutoMapper.Mapper.Map<T>(source);
        }


        
    }
}
