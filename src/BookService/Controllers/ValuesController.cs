using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LaunchDarkly.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BookService.Controllers
{
    [Route("api/BookData/Values")]
    public class ValuesController : Controller
    {
		private const string FF_DelayAmount = "values-inject-delay";
		private const string FF_FailurePercentage = "values-inject-failure";
		LdClient ldClient;
		public ValuesController(LdClient _ldClient)
		{
			ldClient = _ldClient ?? throw new ArgumentNullException(nameof(_ldClient));
		}

        // GET api/values
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
        public IActionResult Probe()
        {
            return Ok();
        }

    }
}
