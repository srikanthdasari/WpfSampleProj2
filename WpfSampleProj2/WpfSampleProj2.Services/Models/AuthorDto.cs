using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WpfSampleProj2.Services.Models
{
    public class AuthorDto
    {
        public Guid guid { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string Genre { get; set; }
    }
}
