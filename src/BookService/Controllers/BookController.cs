using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookService.Model;
using LaunchDarkly.Client;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Controllers
{
    [Produces("application/json")]
    [Route("api/BookData/Book/")]
    public class BookController : Controller
    {
        private const string FF_DelayAmount = "values-inject-delay";
        private const string FF_FailurePercentage = "values-inject-failure";
        LdClient ldClient;

        public BookController(LdClient _ldClient)
        {
            ldClient = _ldClient ?? throw new ArgumentNullException(nameof(_ldClient));
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<Book>> BookList()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
			var ldUser = LaunchDarkly.Client.User.WithKey("FakeUser");
			var delayMs = ldClient.FloatVariation(FF_DelayAmount, ldUser, 0);
			var failurePerc = ldClient.FloatVariation(FF_FailurePercentage, ldUser, 0);
			if (delayMs > 0)
			{
				await Task.Delay((int)delayMs);
			}
			if (failurePerc > 0)
			{
				Random random = new Random();
				int rand = random.Next(1, 100);
                if (rand <= failurePerc)
                {
                    Console.WriteLine($"Failing method, user, {stopwatch.ElapsedMilliseconds} ");
                    throw new ArgumentException("Injected failure");
                }
			}

            stopwatch.Stop();
            Console.WriteLine($"Returning result, user, {stopwatch.ElapsedMilliseconds}");
            return new List<Book>()
            {
                new Book {Id = 1, Author = "W. Shakespeare", Title = "Romeo and Juliet", Year = 1596, Price = 12},
                new Book {Id = 2, Author = "I. Asimov", Title = "I Robot", Year = 1950, Price = 16},
                new Book {Id = 3, Author = "J. Tolkien", Title = "Lord of the rings", Year = 1955, Price = 12.5m},
                new Book {Id = 4, Author = "D. Brown", Title = "The da vinci code", Year = 2003, Price = 13}
            };
        }

        [HttpGet("[action]/{id}")]
        public async Task<IEnumerable<BookReview>> Reviews(int id)
        {
			var ldUser = LaunchDarkly.Client.User.WithKey("FakeUser");
			var delayMs = ldClient.FloatVariation(FF_DelayAmount, ldUser, 0);
			var failurePerc = ldClient.FloatVariation(FF_FailurePercentage, ldUser, 0);
			if (delayMs > 0)
			{
				await Task.Delay((int)delayMs);
			}
			if (failurePerc > 0)
			{
				Random random = new Random();
				int rand = random.Next(1, 100);
				if (rand <= failurePerc)
					throw new ArgumentException("Injected failure");
			}

			return _bookReviews.Where(x => x.BookId == id).ToList();
        }

        [HttpGet("[action]")]
        public string Health()
        {
            return Environment.GetEnvironmentVariable("HOSTNAME");
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var ldUser = LaunchDarkly.Client.User.WithKey("FakeUser");
            var delayMs = ldClient.FloatVariation(FF_DelayAmount, ldUser, 0);
            var failurePerc = ldClient.FloatVariation(FF_FailurePercentage, ldUser, 0);
            if (delayMs > 0)
            {
                await Task.Delay((int)delayMs);
            }
            if (failurePerc > 0)
            {
                Random random = new Random();
                int rand = random.Next(1, 100);
                if (rand <= failurePerc)
                    throw new ArgumentException("Injected failure");
            }
            return new string[] { "value1", "value2" };
        }

        [HttpGet("[action]")]
        public string Headers()
        {
            string headers = String.Empty;
            foreach (var header in Request.Headers)
                headers += header.Key + "=" + Request.Headers[header.Key] + Environment.NewLine;
            return headers;
        }

        private IEnumerable<BookReview> _bookReviews = new List<BookReview>()
        {
            new BookReview { Id = 1, BookId = 1, Author = "Jon Doe", Description = "An extremely entertaining play by Shakespeare. The slapstick humour is refreshing!", Title = "Fantastic Play", Rating = 4 },
            new BookReview { Id = 2, BookId = 1, Author = "Mark White", Description = "Absolutely fun and entertaining. The play lacks thematic depth when compared to other plays by Shakespeare", Title = "Thrilling", Rating = 5 },
            new BookReview { Id = 3, BookId = 2, Author = "Mark White", Description = "It was fun to read Asimov's description of machine learning from the 40s/50s", Title = "Must Read", Rating = 4 },
            new BookReview { Id = 4, BookId = 2, Author = "Edward Johnson", Description = "It's a relatively short book and will keep you entertained the entire time", Title = "Classic SciFi", Rating = 5 },
            new BookReview { Id = 5, BookId = 3, Author = "Mark White", Description = "I felt that this book was beautiful, Tolkien’s imagination is legendary.", Title = "Legendary", Rating = 5 },
            new BookReview { Id = 6, BookId = 3, Author = "Joe Green", Description = "Tolkien really creates his world from below the ground up", Title = "Amazing", Rating = 5 },
            new BookReview { Id = 7, BookId = 4, Author = "Mark Thompson", Description = "A fanciful tale of secret societies, secret codes, and espionage that keeps you interested until the end", Title = "Fanciful Tale", Rating = 5 },
            new BookReview { Id = 8, BookId = 4, Author = "Henry Kyle", Description = "This book is suspenseful, thought provoking, and above all extremely entertaining", Title = "Great Read", Rating = 5 }
        };
    }

    
}