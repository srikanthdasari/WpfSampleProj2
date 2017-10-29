using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WpfSampleProj2.Services.Services
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
