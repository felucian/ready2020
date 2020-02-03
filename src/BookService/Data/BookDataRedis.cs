using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookService.Model;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BookService.Data
{
    public class BookDataRedis : IBookData
    {

        public List<BookReview> GetBookReviews(int bookId)
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();
            var jsonBookReviews = cache.StringGet($"bookreview:{bookId.ToString()}");
            if (!jsonBookReviews.IsNullOrEmpty)
            {
                var book = JsonConvert.DeserializeObject<BookReview>(jsonBookReviews);
            }

            return null;
        }

        public List<Book> GetBooks()
        {
            List<Book> books = new List<Book>();
            var cache = RedisConnectorHelper.Connection.GetDatabase();
            var jsonBooks = cache.SetScan("book","*"); //TODO verifica
            if (jsonBooks != null && jsonBooks.Count() > 0)
            {
                foreach (var book in jsonBooks)
                {
                    books.Add(JsonConvert.DeserializeObject<Book>(book));
                }
            }
            return books;
        }

        public Book InsertBook(Book book)
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();
            var jsonbook = JsonConvert.SerializeObject(book);
            cache.StringSet($"book:{book.Id}", jsonbook);
            return book;
        }

        public void PurgeCollection()
        {
            throw new NotImplementedException();
        }
    }

    public class RedisConnectorHelper
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection;
        static RedisConnectorHelper()
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDISSERVER"));
            });
        }

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}
