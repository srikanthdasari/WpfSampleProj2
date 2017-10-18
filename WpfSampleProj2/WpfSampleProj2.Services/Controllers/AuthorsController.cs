using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfSampleProj2.Services.Models;
using WpfSampleProj2.Services.Services;

namespace WpfSampleProj2.Services.Controllers
{
    [Route("api/[controller]")]
    public class AuthorsController:Controller
    {
        private ILibraryRepository _libraryRepository;

        public AuthorsController(ILibraryRepository repository)
        {
            _libraryRepository = repository;
        }

        [HttpGet]        
        public IActionResult GetAuthors()
        {
            var authorsFromRepo = _libraryRepository.GetAuthors();

            var authors = new List<AuthorDto>();


            return new JsonResult(authorsFromRepo);
        }
    }
}
