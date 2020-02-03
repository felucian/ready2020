using BookService.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BookService.Data
{
    public class BookDataSql : IBookData
    {
        private readonly string GetBooksStored = "proc_GetBooks";
        private readonly string InsertBookStored = "proc_InsertBook";
        private string _currentSchemaVersion;

        public BookDataSql(string currentSchemaVersion)
        {
            _currentSchemaVersion = currentSchemaVersion;
            switch (currentSchemaVersion)
            {
                case SchemaVersions.v1_0_0:
                    GetBooksStored = "proc_GetBooks";
                    InsertBookStored = "proc_InsertBook";
                    break;

                case SchemaVersions.v1_0_1:
                    GetBooksStored = "proc_GetBooks_1_0_1";
                    InsertBookStored = "proc_InsertBook_1_0_1";
                    break;

                default:
                    throw new ArgumentException($"Can't map the {currentSchemaVersion} schema version");
            }
        }

        public List<Book> GetBooks ()
        {
            List<Book> books = new List<Book>();

            using (SqlConnection connection = new SqlConnection(Startup.ConnectionString))
            {
                SqlCommand command = new SqlCommand(GetBooksStored, connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Book book = new Book();
                        book.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        book.Author = reader.GetString(reader.GetOrdinal("Author"));
                        book.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                        book.Title = reader.GetString(reader.GetOrdinal("Title"));
                        book.Year = reader.GetInt16(reader.GetOrdinal("Year"));
                        if (_currentSchemaVersion == SchemaVersions.v1_0_1)
                        {
                            book.Pages = reader.IsDBNull(reader.GetOrdinal("Pages")) ? (int?)null: reader.GetInt16(reader.GetOrdinal("Pages")) ;
                            book.IsValidForGift = reader.IsDBNull(reader.GetOrdinal("IsValidForGift")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("IsValidForGift"));
                        }
                        books.Add(book);
                    }
                }
                reader.Close();
            }
            return books;
        }

        public List<BookReview> GetBookReviews(int bookId)
        {
            List<BookReview> bookReviews = new List<BookReview>();

            using (SqlConnection connection = new SqlConnection(Startup.ConnectionString))
            {
                SqlCommand command = new SqlCommand("proc_GetBookReviews", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("BookId", bookId));
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        bookReviews.Add(new BookReview()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                            Author = reader.GetString(reader.GetOrdinal("Author")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Rating = reader.GetByte(reader.GetOrdinal("Rating"))
                        });
                    }
                }
                reader.Close();
            }
            return bookReviews;
        }
        
        public Book InsertBook(Book book)
        {
            using (SqlConnection connection = new SqlConnection(Startup.ConnectionString))
            {
                SqlCommand command = new SqlCommand(InsertBookStored, connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("Title", book.Title));
                command.Parameters.Add(new SqlParameter("Author", book.Author));
                command.Parameters.Add(new SqlParameter("Year", book.Year));
                command.Parameters.Add(new SqlParameter("Price", book.Price));
                if (_currentSchemaVersion == SchemaVersions.v1_0_1)
                {
                    command.Parameters.Add(new SqlParameter("Pages", book.Pages));
                    command.Parameters.Add(new SqlParameter("IsValidForGift", book.IsValidForGift));
                }
                var bookid = command.Parameters.Add("@ID", SqlDbType.Int, 4);
                bookid.Direction = ParameterDirection.Output;
                
                connection.Open();
                command.ExecuteNonQuery();
                book.Id = (int) bookid.Value;

            }

            return book;
        }
    }


}
