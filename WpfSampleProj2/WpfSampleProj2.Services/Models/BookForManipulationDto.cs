using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WpfSampleProj2.Services.Models
{
    public abstract class BookForManipulationDto
    {
        [Required(ErrorMessage = "This is mandatory")]
        [MaxLength(100, ErrorMessage = "Max 100 chars allowed")]
        public virtual string Title { get; set; }

        [MaxLength(500, ErrorMessage = "Max 500 chars allowed")]
        public virtual string Description { get; set; }
    }
}
