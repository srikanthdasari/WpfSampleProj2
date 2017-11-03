using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WpfSampleProj2.Services.Models
{
    public class LinkCollectionResourceWrapperDto<T>:LinkResourceBaseDto
        where T:LinkResourceBaseDto
    {
        public IEnumerable<T> Value { get; set; }


        public LinkCollectionResourceWrapperDto(IEnumerable<T> value)
        {
            Value = value;
        }
    }
}
