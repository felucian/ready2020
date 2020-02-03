using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookService.Controllers
{
	[Route("api/[controller]")]
	public class WarmupController : Controller
	{
		private static bool isWarm = false;
		private static HttpClient _httpClient = new HttpClient();

		// GET: api/<controller>
		[HttpGet, HttpHead]
		public IActionResult Get()
		{
			if (!isWarm)
			{
				var formContent = new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string,string>("username","a_fake_username")
				};

				var tasks = new List<Task>();
				tasks.AddRange(new List<Task>() {
					_httpClient.GetAsync(GetUrl(Url.Action("BookList","Book"))),
					_httpClient.GetAsync(GetUrl(Url.Action("Reviews","Book",new { id=1 }))),
					_httpClient.GetAsync(GetUrl(Url.Action("Health", "Book"))),
					_httpClient.GetAsync(GetUrl(Url.Action("Get", "Book"))),
					_httpClient.GetAsync(GetUrl(Url.Action("Headers", "Book"))),
					_httpClient.GetAsync(GetUrl(Url.Action("Get", "Values"))),
					_httpClient.GetAsync(GetUrl(Url.Action("Probe", "Values"))),

					}
				);

				Task.WaitAll(tasks.ToArray());
				isWarm = true;
			}
			return Ok("READY");

		}

		private string GetUrl(string actionUrl)
		{
			return $"{Request.Scheme}://{Request.Host}{actionUrl}";
		}
	}
}
