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
using WpfSampleProj2.Services.Helper;

namespace WpfSampleProj2.Services.Controllers
{
    [Route("api/[controller]")]
    public class AuthorsController:Controller
    {
        private ILibraryRepository _libraryRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public AuthorsController(ILibraryRepository repository, 
            IUrlHelper urlHelper, 
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _libraryRepository = repository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name ="GetAuthors")]        
        public IActionResult GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {

            if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(authorsResourceParameters.OrderBy))
                return BadRequest();

            if(!_typeHelperService.TypeHasProperties<AuthorDto>(authorsResourceParameters.Fields))
            {
                return BadRequest();
            }

            var authorsFromRepo = _libraryRepository.GetAuthors(authorsResourceParameters);

            var previousPageLink = authorsFromRepo.HasPrevious ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage) : null;

            var nextPageLink = authorsFromRepo.HasNext ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize=authorsFromRepo.PageSize,
                currentPage=authorsFromRepo.CurrentPage,
                totalPages=authorsFromRepo.TotalPages,
                previousPageLink=previousPageLink,
                nextPageLink=nextPageLink
            };


            Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var authors = authorsFromRepo.Map<IEnumerable<AuthorDto>>();

            return Ok(authors.ShapeData(authorsResourceParameters.Fields));
        }

        private string CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters, ResourceUriType type)
        {
            switch(type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetAuthors", new
                    {
                        fields=authorsResourceParameters.Fields,
                        orderBy=authorsResourceParameters.OrderBy,
                        searchQuery = authorsResourceParameters.SearchQuery,
                        genre=authorsResourceParameters.Genre,
                        pageNumber = authorsResourceParameters.PageNumber - 1,
                        pageSize=authorsResourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetAuthors", new
                    {
                        fields = authorsResourceParameters.Fields,
                        orderBy = authorsResourceParameters.OrderBy,
                        searchQuery = authorsResourceParameters.SearchQuery,
                        genre = authorsResourceParameters.Genre,
                        pageNumber = authorsResourceParameters.PageNumber + 1,
                        pageSize=authorsResourceParameters.PageSize
                    });
                default:
                    return _urlHelper.Link("GetAuthors", new
                    {
                        fields = authorsResourceParameters.Fields,
                        orderBy = authorsResourceParameters.OrderBy,
                        searchQuery = authorsResourceParameters.SearchQuery,
                        genre = authorsResourceParameters.Genre,
                        pageNumber =authorsResourceParameters.PageNumber,
                        pageSize=authorsResourceParameters.PageSize
                    });
            }
        }


        [HttpGet("{id}", Name ="GetAuthor")]
        public IActionResult GetAuthor(Guid id,[FromQuery] string fields)
        {
            Contract.Ensures(Contract.Result<IActionResult>() != null);
            AuthorDto author = null;

            if (!_typeHelperService.TypeHasProperties<AuthorDto>(fields))
                return BadRequest();

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
                return Ok(author.ShapeData(fields));
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

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(Guid id)
        {
            var authorFromRepo = _libraryRepository.GetAuthor(id);
            if(authorFromRepo.IsNull())
            {
                return NotFound();
            }

            _libraryRepository.DeleteAuthor(authorFromRepo);

            if(!_libraryRepository.Save())
            {
                throw new Exception($"Error on deleting the {id}");
            }

            return NoContent();
        }

    }
}
