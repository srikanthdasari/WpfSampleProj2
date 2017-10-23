using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfSampleProj2.Lib.Extensions;
using WpfSampleProj2.Lib.Helper;
using WpfSampleProj2.Services.Entities;
using WpfSampleProj2.Services.Extensions;
using WpfSampleProj2.Services.Models;
using WpfSampleProj2.Services.Services;

namespace WpfSampleProj2.Services.Controllers
{
    [Route("api/authorcollections")]
    public class AuthorCollectionController:Controller
    {

        private ILibraryRepository _libraryRepository;

        public AuthorCollectionController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpPost]
        public IActionResult CreatAuthoCollection([FromBody] IEnumerable<AuthorForCreationDto> authorCollection)
        {
            if(authorCollection.IsNull())
            {
                return BadRequest();
            }

            var authorEntities = authorCollection.Map<IEnumerable<Author>>();

            foreach (var author in authorEntities)
            {
                _libraryRepository.AddAuthor(author);
            }


            if(!_libraryRepository.Save())
            {
                throw new Exception("Creating and Author collection failed to Save");
            }

            var authorCollectionToReturn = authorEntities.Map<IEnumerable<AuthorDto>>();
            var idsAsString = string.Join(",", authorCollectionToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetAuthorCollection", new { ids = idsAsString }, authorCollectionToReturn);

            //return Ok();
        }

        [HttpGet("({ids})", Name ="GetAuthorCollection")]
        public IActionResult GetAuthorCollection([ModelBinder(BinderType =typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            if(ids.IsNull())
            {
                return BadRequest();
            }

            var authorEntities = _libraryRepository.GetAuthors(ids);

            if(ids.Count()!=authorEntities.Count())
            {
                return NotFound();
            }

            var authorsToReturn = authorEntities.Map<AuthorDto>();

            return Ok(authorsToReturn);
        }


       
    }
}
