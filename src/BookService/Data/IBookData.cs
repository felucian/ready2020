using BookService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookService.Data
{
    public interface IBookData
    {
        List<Book> GetBooks();
        List<BookReview> GetBookReviews(int bookId);
        Book InsertBook(Book book);
    }
}
