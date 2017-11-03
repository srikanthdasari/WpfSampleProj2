using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WpfSampleProj2.Services.Models
{
    public abstract class LinkResourceBaseDto
    {
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}
