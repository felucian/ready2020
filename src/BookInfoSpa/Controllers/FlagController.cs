using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaunchDarkly.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookInfoSpa.Controllers
{
    [Route("api/[controller]")]
    public class FlagController : Controller
    {
        [HttpGet("[action]/{flag}")]
        public bool Get(string flag)
        {
            var currentUser = HttpContext.Session.GetString(SessionConstant.User);
            User user = LaunchDarkly.Client.User.WithKey(currentUser);
            var res = Startup.FeatureFlag.BoolVariation(flag, user, false);
            return res;
        }

        [HttpGet("[action]")]
        public string GetKey(string flag)
        {
            return Environment.GetEnvironmentVariable("FEATUREFLAG_KEY");
        }
    }


}