using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfSampleProj2.Services.Services;
using System.Linq.Dynamic.Core;
using WpfSampleProj2.Lib.Extensions;

namespace WpfSampleProj2.Services.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySoft<T>(this IQueryable<T> source, string orderBy,
            Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source.IsNull())
                throw new ArgumentException("Source");

            if (mappingDictionary.IsNull())
                throw new ArgumentException("MappingDcitionary");

            if (orderBy.IsEmpty())
                return source;

            var orderByAfterSplit = orderBy.Split(",");

            foreach(var orderByClause in orderByAfterSplit.Reverse())
            {
                var trimmedOrderByClause = orderBy.Trim();

                var orderDescending = trimmedOrderByClause.EndsWith(" desc", StringComparison.Ordinal);

                var indexOffFirstSpace = trimmedOrderByClause.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOffFirstSpace == -1 ? trimmedOrderByClause : trimmedOrderByClause.Remove(indexOffFirstSpace);

                if(!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");
                }

                var propertyMappingValue = mappingDictionary[propertyName];

                if(propertyMappingValue.IsNull())
                {
                    throw new ArgumentException("PropertyMappingValue");
                }

                foreach (var destinationProperty in propertyMappingValue.DestinationProperties.Reverse())
                {
                    if(propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }
                    source = source.OrderBy(destinationProperty + (orderDescending ? " descending" : " ascending"));
                }

            }

            return source;
        }
    }
}
