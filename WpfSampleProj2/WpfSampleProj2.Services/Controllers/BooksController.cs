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
using Microsoft.AspNetCore.JsonPatch;

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

        [HttpDelete("{id}")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
        {
            if(!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);

            if(bookForAuthorFromRepo.IsNull())
            {
                return NotFound();
            }

            _libraryRepository.DeleteBook(bookForAuthorFromRepo);

            if(!_libraryRepository.Save())
            {
                throw new Exception($"Deletingbook {id} for author{authorId} failed on");
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id, [FromBody] BookForUpdateDto book)
        {
            if(book.IsNull())
            {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);

            if (bookForAuthorFromRepo.IsNull())
            {
                var bookToAdd = book.Map<Book>();
                bookToAdd.Id = id;
                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!_libraryRepository.Save())
                {
                    throw new Exception($"Updating book {id} for author {authorId} failed on save.");
                }

                var bookToReturn = bookToAdd.Map<BookDto>();

                return CreatedAtRoute("GetBooksForAuthor", new { authorId = authorId, id = bookToReturn.Id }, bookToReturn);
            }

            bookForAuthorFromRepo.UpdateDestination<Book, BookForUpdateDto>(book);
            
            _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if(!_libraryRepository.Save())
            {
                throw new Exception($"Updating book {id} for author {authorId} failed on update.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid id, [FromBody] JsonPatchDocument<BookForUpdateDto> patchDoc)
        {
            if (patchDoc.IsNull())
                return BadRequest();

            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);

            if (bookForAuthorFromRepo.IsNull())
            {
                var bookDto = new BookForUpdateDto();
                patchDoc.ApplyTo(bookDto);

                var bookToAdd = bookDto.Map<Book>();

                bookToAdd.Id = id;

                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!_libraryRepository.Save())
                    throw new Exception($"Upserting book {id} for author {authorId} failed on upserting");

                var bookToreturn = bookToAdd.Map<BookDto>();

                return CreatedAtRoute("GetBooksForAuthor", new { authorId = authorId, id = bookToreturn.Id }, bookToreturn);
            }

            var bookToPatch = bookForAuthorFromRepo.Map<BookForUpdateDto>();

            patchDoc.ApplyTo(bookToPatch);

            // Add Validation

            bookForAuthorFromRepo.UpdateDestination<Book, BookForUpdateDto>(bookToPatch);

            _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if(!_libraryRepository.Save())
            {
                throw new Exception($"Patching book {id} for author {authorId} failed on update");
            }

            return NoContent();
        }
    }
}
