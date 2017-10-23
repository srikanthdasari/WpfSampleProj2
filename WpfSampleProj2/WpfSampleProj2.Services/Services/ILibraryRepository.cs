using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfSampleProj2.Services.Entities;

namespace WpfSampleProj2.Services.Services
{
    public interface ILibraryRepository
    {
        IEnumerable<Author> GetAuthors();

        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);

        Author GetAuthor(Guid AuthorId);
        
        void AddAuthor(Author author);

        void DeleteAuthor(Author author);

        bool AuthorExists(Guid authoId);

        IEnumerable<Book> GetBooksForAuthor(Guid authorId);

        Book GetBookForAuthor(Guid authorId, Guid bookId);

        Book GetBookbyBookId(Guid bookId);

        bool BookExists(Guid bookId);

        void AddBookForAuthor(Guid authorId, Book book);

        void UpdateBookForAuthor(Book book);

        void DeleteBook(Book book);

        bool Save();
    }
}
