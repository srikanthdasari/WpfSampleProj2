using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WpfSampleProj2.Services.Extensions
{
    public static class MapperExtension
    {
        public static T Map<T>(this object source)
        {
            return AutoMapper.Mapper.Map<T>(source);
        }


        public static TDest UpdateDestination<TDest,TSource>(this object dest, TSource source)
        {
            return (TDest)Mapper.Map(source, dest , typeof(TSource), typeof(TDest));
        }


        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>
            (this IMappingExpression<TSource, TDestination> expression)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var sourceType = typeof(TSource);
            var destinationProperties = typeof(TDestination).GetProperties(flags);

            foreach (var property in destinationProperties)
            {
                if (sourceType.GetProperty(property.Name, flags) == null)
                {
                    expression.ForMember(property.Name, opt => opt.Ignore());
                }
            }
            return expression;
        }

    }
}
