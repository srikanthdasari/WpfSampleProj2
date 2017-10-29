using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfSampleProj2.Lib.Extensions;
using WpfSampleProj2.Services.Entities;
using WpfSampleProj2.Services.Models;

namespace WpfSampleProj2.Services.Services
{
    public class PropertyMappingService: IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _authorPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>(){"Id"}) },
                {"Genre", new PropertyMappingValue(new List<string>(){"Genre"}) },
                {"Age", new PropertyMappingValue(new List<string>(){"DateOfBirth"},true) },
                {"Name", new PropertyMappingValue(new List<string>() {"FirstName","LastName"}) }
            };


        private IList<IPropertyMapping> propertyMapping = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMapping.Add(new PropertyMapping<AuthorDto, Author>(_authorPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource,TDestination>()
        {
            var matchingMapping = propertyMapping.OfType<PropertyMapping<TSource, TDestination>>();

            if(matchingMapping.Count()==1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance for <{typeof(TSource)}> ");
        }


        public bool ValidMappingExistsFor<TSource,TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (fields.IsEmpty())
                return true;

            var fieldsAfterSplit = fields.Split(',');

            foreach(var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                var indexOfFirstSpace = trimmedField.IndexOf(" ", StringComparison.Ordinal);

                var propertyName = indexOfFirstSpace == -1 ? trimmedField : trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                    return false;
            }

            return true;
        }
    }
}
