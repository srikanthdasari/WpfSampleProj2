﻿using AutoMapper;
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
using WpfSampleProj2.Lib.Helper;
using Microsoft.Extensions.Logging;

namespace WpfSampleProj2.Services.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController:Controller
    {
        private ILibraryRepository _libraryRepository;
        private ILogger _logger;
        private IUrlHelper _urlHelper;

        public BooksController(ILibraryRepository libraryRepository, ILogger<BooksController> logger,IUrlHelper urlHelper)
        {
            _libraryRepository = libraryRepository;
            _logger = logger;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name ="GetBooksForAuthor")]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            if(!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksForAuthorFromRepo = _libraryRepository.GetBooksForAuthor(authorId);

            var bookForAuthor = booksForAuthorFromRepo.IfNotNull(x=>Mapper.Map<IEnumerable<BookDto>>(x));
            if (bookForAuthor.IsNull()) return NotFound();

            bookForAuthor = bookForAuthor.Select(book =>
              {
                  book = CreateLinksForBooks(book);
                  return book;
              });

            var wrapper = new LinkCollectionResourceWrapperDto<BookDto>(bookForAuthor);

            return Ok(CreateLinksForBooks(wrapper));
        }

        [HttpGet("{id}", Name ="GetBookForAuthor")]
        public IActionResult GetBooksForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);

            if (bookForAuthorFromRepo.IsNull()) return NotFound();

            var bookForAuthor = bookForAuthorFromRepo.Map<BookDto>();

            return Ok(CreateLinksForBooks(bookForAuthor));
        }

        [HttpPost(Name ="CreateBookForAuthor")]
        public IActionResult CreateBookForAuthor(Guid authorId,[FromBody] BookForCreationDto book)
        {
            if(book.IsNull())
            {
                return BadRequest();
            }

            if(book.Description==book.Title)
            {
                ModelState.AddModelError(nameof(BookForCreationDto), "The Provided description should be different from the title.");
            }

            if(!ModelState.IsValid)
            {
                //Return 422
                return new UnprocessableEntityObjectResult(ModelState);
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

            return CreatedAtRoute("GetBooksForAuthor",new { authorId, id = bookToreturn.Id }, CreateLinksForBooks(bookToreturn));
        }

        [HttpDelete("{id}", Name ="DeleteBookForAuthor")]
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

            _logger.LogInformation(100, $"Bok {id} for author {authorId} was deleted");

            return NoContent();
        }

        [HttpPut("{id}", Name ="UpdateBookForAuthor")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id, [FromBody] BookForUpdateDto book)
        {
            if(book.IsNull())
            {
                return BadRequest();
            }

            if (book.Description == book.Title)
            {
                ModelState.AddModelError(nameof(BookForUpdateDto), "The Provided description should be different from the title.");
            }

            if (!ModelState.IsValid)
            {
                //Return 422
                return new UnprocessableEntityObjectResult(ModelState);
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

        [HttpPatch("{id}", Name ="PartiallyUpdateBookForAuthor")]
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

            patchDoc.ApplyTo(bookToPatch, ModelState);

            if(!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            bookForAuthorFromRepo.UpdateDestination<Book, BookForUpdateDto>(bookToPatch);

            _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if(!_libraryRepository.Save())
            {
                throw new Exception($"Patching book {id} for author {authorId} failed on update");
            }

            return NoContent();
        }


        private BookDto CreateLinksForBooks(BookDto book)
        {
            book.Links.Add(new LinkDto(_urlHelper.Link("GetBookForAuthor", new { id = book.Id }), "self", "GET"));
            book.Links.Add(new LinkDto(_urlHelper.Link("DeleteBookForAuthor", new { id = book.Id }), "delete_book", "DELETE"));
            book.Links.Add(new LinkDto(_urlHelper.Link("UpdateBookForAuthor", new { id = book.Id }), "update_book", "PUT"));
            book.Links.Add(new LinkDto(_urlHelper.Link("PartiallyUpdateBookForAuthor", new { id = book.Id }), "partially_update_book", "PATCH"));

            return book;
        }

        private LinkCollectionResourceWrapperDto<BookDto> CreateLinksForBooks(LinkCollectionResourceWrapperDto<BookDto> booksWrapper)
        {

            booksWrapper.Links.Add(new LinkDto(_urlHelper.Link("GetBooksForAuthor", new {  }), "self", "GET"));
            return booksWrapper;
        }

    }
}
