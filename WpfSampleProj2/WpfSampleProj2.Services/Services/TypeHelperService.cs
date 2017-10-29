using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WpfSampleProj2.Lib.Extensions;

namespace WpfSampleProj2.Services.Services
{
    public class TypeHelperService: ITypeHelperService
    {
        public bool TypeHasProperties<T>(string fields)
        {
            if (fields.IsNull())
                return true;

            var fieldsAfterSplit = fields.Split(',');

            foreach(var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();

                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if(propertyInfo.IsNull())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
