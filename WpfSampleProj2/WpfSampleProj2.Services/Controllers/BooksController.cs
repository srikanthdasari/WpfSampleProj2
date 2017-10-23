using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfSampleProj2.Lib.Extensions;
using WpfSampleProj2.Services.Models;
using WpfSampleProj2.Services.Services;
using WpfSampleProj2.Services.Extensions;
using WpfSampleProj2.Services.Entities;

namespace WpfSampleProj2.Services.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController:Controller
    {
        private ILibraryRepository _libraryRepository;

        public BooksController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet(Name = "GetBooksForAuthor")]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            if(!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksForAuthorFromRepo = _libraryRepository.GetBooksForAuthor(authorId);

            var bookForAuthor = booksForAuthorFromRepo.IfNotNull(x=>Mapper.Map<IEnumerable<BookDto>>(x));
            if (bookForAuthor.IsNull()) return NotFound();
            return Ok(bookForAuthor);
        }

        [HttpPost]
        public IActionResult CreateBookForAuthor(Guid authorId,[FromBody] BookForCreationDto book)
        {
            if(book.IsNull())
            {
                return BadRequest();
            }

            if(!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookEntity = book.Map<Book>();

            _libraryRepository.AddBookForAuthor(authorId, bookEntity); 

            if(!_libraryRepository.Save())
            {
                throw new Exception($"Creating a book for Author {authorId} failed on Save.");
            }

            var bookToreturn = bookEntity.Map<BookDto>();

            return CreatedAtRoute("GetBooksForAuthor",new { authorId, id = bookToreturn.Id }, bookToreturn);
        }

        //public IActionResult GetBookByBookId(Guid bookId)
        //{
        //    if(!_libraryRepository.)
        //}
    }
}
