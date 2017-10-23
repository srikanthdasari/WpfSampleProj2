using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfSampleProj2.Lib.Extensions;
using WpfSampleProj2.Services.Extensions;
using WpfSampleProj2.Services.Models;
using WpfSampleProj2.Services.Services;
using System.Diagnostics.Contracts;
using WpfSampleProj2.Lib.Helper;
using WpfSampleProj2.Services.Entities;
using Microsoft.AspNetCore.Http;

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
            
            return Ok(authorsFromRepo.Map<IEnumerable<AuthorDto>>());
        }


        [HttpGet("{id}", Name ="GetAuthor")]
        public IActionResult GetAuthor(Guid id)
        {
            Contract.Ensures(Contract.Result<IActionResult>() != null);
            AuthorDto author = null;

            if (!_libraryRepository.AuthorExists(id))
                return NotFound();

            TryCatchHelper.Execute(() =>
            {
                var authorFromRepo = _libraryRepository.GetAuthor(id);
                author = authorFromRepo.Map<AuthorDto>();                

            }).IfNotNull(ex =>
            {
                return StatusCode(500, "An Unhandled fault Happend. Try again later");
            });


            if (author.IsNotNull())
                return Ok(author);
            return NotFound();
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorForCreationDto author)
        {
            if(author.IsNull())
            {
                return BadRequest();
            }

            var authorEntity = author.Map<Author>();  

            _libraryRepository.AddAuthor(authorEntity);

            if(!_libraryRepository.Save())
            {
                return StatusCode(500, "A problem happend with handling your request.");
            }

            var authorToReturn = authorEntity.Map<AuthorDto>();

            return CreatedAtRoute("GetAuthor", new { id = authorToReturn.Id }, authorToReturn);
        }


        [HttpPost("{id}")]
        public IActionResult BlockAuthorCreation(Guid id)
        {
            if (_libraryRepository.AuthorExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }


    }
}
