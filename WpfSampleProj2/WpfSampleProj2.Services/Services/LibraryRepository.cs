using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WpfSampleProj2.Lib.Extensions;
using WpfSampleProj2.Services.Entities;
using WpfSampleProj2.Services.Extensions;
using WpfSampleProj2.Services.Helper;
using WpfSampleProj2.Services.Models;

namespace WpfSampleProj2.Services.Services
{
    public class LibraryRepository : ILibraryRepository
    {

        private LibraryContext _context;
        private IPropertyMappingService _propertyMappingService;

        public LibraryRepository(LibraryContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public void AddAuthor(Author author)
        {
            author.Id = Guid.NewGuid();
            _context.Authors.Add(author);

            //the repository fills the id (instead of using identity column)
            if (author.Books.IfNotNull(x=>x.Any()))
            {
                foreach (var book in author.Books)
                {
                    book.Id = Guid.NewGuid();
                }
            }
        }

        public void AddBookForAuthor(Guid authorId, Book book)
        {
            var author = GetAuthor(authorId);
            if (author.IsNotNull())
            {
                if (book.Id.IsNull())
                {
                    book.Id = Guid.NewGuid();
                }

                if (author.Books.IsNull())
                    author.Books = new Collection<Book>();
                author.Books.Add(book);
            }
        }

        public bool AuthorExists(Guid authoId)
        {
            return _context.Authors.Any(a => a.Id == authoId);
        }

        public bool BookExists(Guid bookId)
        {
            return _context.Books.Any(a => a.Id == bookId);
        }

        public void DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public Author GetAuthor(Guid AuthorId)
        {
            return _context.Authors.FirstOrDefault(x => x.Id == AuthorId);
        }

        public PagedList<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            //var list = _context.Authors
            //    .OrderBy(a => a.FirstName)
            //    .ThenBy(a => a.LastName).AsQueryable();
            //.Skip(authorsResourceParameters.PageSize * (authorsResourceParameters.PageNumber - 1))
            //.Take(authorsResourceParameters.PageSize)
            //.ToList();

            var list = _context.Authors.ApplySoft(authorsResourceParameters.OrderBy, 
                _propertyMappingService.GetPropertyMapping<AuthorDto, Author>());
                
            if(authorsResourceParameters.Genre.IsNotEmpty())
            {
                var genreFromWhereClause = authorsResourceParameters.Genre.Trim().ToLowerInvariant();

                list = list.Where(z => z.Genre.ToLowerInvariant() == genreFromWhereClause);
            }

            if(authorsResourceParameters.SearchQuery.IsNotEmpty())
            {
                var searchQueryForWhereClause = authorsResourceParameters.SearchQuery.Trim().ToLowerInvariant();

                list = list.Where(a => a.Genre.ToLowerInvariant().Contains(searchQueryForWhereClause)
                || a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                || a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return PagedList<Author>.Create(list, authorsResourceParameters.PageNumber, authorsResourceParameters.PageSize);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }


        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            return _context.Books
                        .Where(b => b.AuthorId == authorId).OrderBy(b => b.Title).ToList();


        }

        public Book GetBookbyBookId(Guid bookId)
        {
            return _context.Books.Where(b => b.Id == bookId).FirstOrDefault();
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            return _context.Books
              .Where(b => b.AuthorId == authorId && b.Id == bookId).FirstOrDefault();
        }


        public bool Save()
        {
            return (_context.SaveChanges() >= 0);

        }

        public void UpdateBookForAuthor(Book book)
        {
            //
        }
    }
}
