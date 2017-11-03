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
using AutoMapper;

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
        public IActionResult GetAuthors(AuthorsResourceParameters authorsResourceParameters,[FromHeader(Name="Accept")] string mediaType)
        {

            if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(authorsResourceParameters.OrderBy))
                return BadRequest();

            if(!_typeHelperService.TypeHasProperties<AuthorDto>(authorsResourceParameters.Fields))
            {
                return BadRequest();
            }

            var authorsFromRepo = _libraryRepository.GetAuthors(authorsResourceParameters);
            
            

            var authors = authorsFromRepo.Map<IEnumerable<AuthorDto>>();
            if (mediaType == "application/vnd.marvin.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = authorsFromRepo.TotalCount,
                    pageSize = authorsFromRepo.PageSize,
                    currentPage = authorsFromRepo.CurrentPage,
                    totalPages = authorsFromRepo.TotalPages,
                
                };

                Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateLinksForAuthors(authorsResourceParameters, authorsFromRepo.HasNext, authorsFromRepo.HasPrevious);

                var shapedAuthors = authors.ShapeData(authorsResourceParameters.Fields);

                var shapedAuthorsWithLinks = shapedAuthors.Select(author =>
                  {
                      var authorAsDictionary = author as IDictionary<string, object>;

                      var authorLinks = CreateLinksForAuthor((Guid)authorAsDictionary["Id"], authorsResourceParameters.Fields);
                      authorAsDictionary.Add("links", authorLinks);

                      return authorAsDictionary;
                  });


                var linkedCollectionResource = new
                {
                    value = shapedAuthorsWithLinks,
                    links = links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousPageLink = authorsFromRepo.HasPrevious ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage) : null;

                var nextPageLink = authorsFromRepo.HasNext ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = authorsFromRepo.TotalCount,
                    pageSize = authorsFromRepo.PageSize,
                    currentPage = authorsFromRepo.CurrentPage,
                    totalPages = authorsFromRepo.TotalPages,
                    previousPageLink = previousPageLink,
                    nextPageLink = nextPageLink
                };

                Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                return Ok(authors.ShapeData(authorsResourceParameters.Fields));

            }
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
                case ResourceUriType.Current:
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
            {
                var links = CreateLinksForAuthor(id, fields);

                var linkedResourceToReturn = author.ShapeData(fields) as IDictionary<string, object>;

                linkedResourceToReturn.Add("links", links);

                return Ok(linkedResourceToReturn);
            }
            return NotFound();
        }

        [HttpPost(Name ="CreateAuthor")]
        [RequestHeaderMatchesMediaType("Content-Type", new[] {"application/vnd.marvin.author.full+json"})]
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

            var links = CreateLinksForAuthor(authorToReturn.Id, null);
            var linkedResourceToReturn = authorToReturn.ShapeData(null) as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetAuthor", new { id = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
        }

        [HttpPost(Name = "CreateAuthorWithDateOfDeath")]
        [RequestHeaderMatchesMediaType("Content-Type",new[] { "application/vnd.marvin.authorwithdateofdeath.full+json" })]
        public IActionResult AuthorForCreationWithDateOfDeathDto([FromBody] AuthorForCreationWithDateOfDeathDto author)
        {
            if (author.IsNull())
            {
                return BadRequest();
            }


            var authorEntity = new Author();
            authorEntity.UpdateDestination<Author, AuthorForCreationWithDateOfDeathDto>(author);

            _libraryRepository.AddAuthor(authorEntity);

            if (!_libraryRepository.Save())
            {
                return StatusCode(500, "A problem happend with handling your request.");
            }

            var authorToReturn = authorEntity.Map<AuthorDto>();

            var links = CreateLinksForAuthor(authorToReturn.Id, null);
            var linkedResourceToReturn = authorToReturn.ShapeData(null) as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetAuthor", new { id = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
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

        [HttpDelete("{id}", Name ="DeleteAuthor" )]
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


        public IEnumerable<LinkDto> CreateLinksForAuthor(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if(fields.IsEmpty())
            {
                links.Add(new LinkDto(_urlHelper.Link("GetAuthor", new { id = id }), "self", "GET"));
            }
            else
            {
                links.Add(new LinkDto(_urlHelper.Link("GetAuthor", new { id = id, fields = fields }), "self", "GET"));
            }

            links.Add(new LinkDto(_urlHelper.Link("DeleteAuthor", new { id = id }), "delete_author", "DELETE"));
            links.Add(new LinkDto(_urlHelper.Link("CreateBookForAuthor", new { authorId = id }), "create_book_for_author", "POST"));
            links.Add(new LinkDto(_urlHelper.Link("GetBooksForAuthor", new { authorId = id }), "books", "GET"));
            return links;
        }


        public IEnumerable<LinkDto> CreateLinksForAuthors(AuthorsResourceParameters authorsResourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.Current), "self", "GET"));

            if(hasNext)
            {
                links.Add(new LinkDto(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage), "nextPage", "GET"));
            }
            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }

    }
}
