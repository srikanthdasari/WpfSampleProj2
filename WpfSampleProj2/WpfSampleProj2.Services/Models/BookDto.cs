using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WpfSampleProj2.Services.Models
{
    public class BookDto:LinkResourceBaseDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Guid AuthorId { get; set; }
    }
}
