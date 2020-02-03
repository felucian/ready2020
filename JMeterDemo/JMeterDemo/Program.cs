using LaunchDarkly.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JMeterDemo
{
    class Program
    {
        static LdClient _ldC = new LdClient(ConfigurationManager.AppSettings["LDKey"]);
        static void Main(string[] args)
        {
            //Run().GetAwaiter().GetResult();
            RunWithStats().GetAwaiter().GetResult();
        }

        static async Task Run()
        {
            //Default connection limit is 2....increase it
            ServicePointManager.DefaultConnectionLimit = 9999;

            HttpClient clientResilient = new HttpClient();
            Uri resilientUri = new Uri(ConfigurationManager.AppSettings["baseAddressResilient"]); //https://stackoverflow.com/a/23438417
            clientResilient.BaseAddress = resilientUri;
            clientResilient.DefaultRequestHeaders.ConnectionClose = false; //connection: keep-alive

            HttpClient clientUnresilient = new HttpClient();
            Uri noResilientUri = new Uri(ConfigurationManager.AppSettings["baseAddressNoResilient"]);//https://stackoverflow.com/a/23438417
            clientUnresilient.BaseAddress = noResilientUri;
            clientUnresilient.DefaultRequestHeaders.ConnectionClose = false; //connection: keep-alive

            //ServicePointManager.FindServicePoint(resilientUri).ConnectionLeaseTimeout = 60 * 1000;
            //ServicePointManager.FindServicePoint(noResilientUri).ConnectionLeaseTimeout = 60 * 1000;

            var requestPath = ConfigurationManager.AppSettings["requestPath"];
            var user = LaunchDarkly.Client.User.WithKey("FakeUser");
            var defaultRPS = int.Parse(ConfigurationManager.AppSettings["DefaultRPS"]);

            for (int i = 0; i < 99999; i++)
            {
                var requestsPerSec = _ldC.FloatVariation("rps", user, defaultRPS);
                Console.WriteLine($"Starting batch {i} with {requestsPerSec} rps:");
                for (int j = 0; j < requestsPerSec; j++)
                {
                    Console.Write($"{j}, ");
                    clientResilient.GetAsync(requestPath);
                    //clientUnresilient.GetAsync(requestPath);
                }

                Console.WriteLine();
                await Task.Delay(1000);
            }

        }

        static async Task RunWithStats()
        {
            //Default connection limit is 2....increase it
            ServicePointManager.DefaultConnectionLimit = 9999;

            HttpClient statsClient = new HttpClient();
            statsClient.BaseAddress = new Uri("http://localhost:30081/");

            HttpClient clientResilient = new HttpClient();
            Uri resilientUri = new Uri(ConfigurationManager.AppSettings["baseAddressResilient"]); //https://stackoverflow.com/a/23438417
            clientResilient.BaseAddress = resilientUri;
            clientResilient.DefaultRequestHeaders.ConnectionClose = false; //connection: keep-alive

            HttpClient clientUnresilient = new HttpClient();
            Uri noResilientUri = new Uri(ConfigurationManager.AppSettings["baseAddressNoResilient"]);//https://stackoverflow.com/a/23438417
            clientUnresilient.BaseAddress = noResilientUri;
            clientUnresilient.DefaultRequestHeaders.ConnectionClose = false; //connection: keep-alive

            ServicePointManager.FindServicePoint(resilientUri).ConnectionLeaseTimeout = 60 * 1000;
            ServicePointManager.FindServicePoint(noResilientUri).ConnectionLeaseTimeout = 60 * 1000;
            var requestPath = ConfigurationManager.AppSettings["requestPath"];

            var user = User.WithKey("FakeUser");
            var rps = int.Parse(ConfigurationManager.AppSettings["DefaultRPS"]);
            

            List<StatsPerBucket> bucketStats = new List<StatsPerBucket>();

            while (true)
            {
                Console.WriteLine("Cleaning stats....");
                Console.WriteLine($"Current RPS = {rps}");
                await statsClient.PostAsync("reset_counters", null);

                for (int i = 0; i < 120000; i++)
                {
                    rps = _ldC.IntVariation("rps", user, rps);
                    Console.WriteLine($"Run {i}");
                    for (int j = 0; j < rps; j++)
                    {
                        Console.Write($"{j}, ");
                        clientResilient.GetAsync(requestPath);
                        clientUnresilient.GetAsync(requestPath);
                    }
                    await Task.Delay(500);
                }

                var sidecarStats = await statsClient.GetAsync("stats");
                var sidecarStatRows = (await sidecarStats.Content.ReadAsStringAsync()).Split('\n');

                Dictionary<string, string> stats = new Dictionary<string, string>();
                StatsPerBucket bucketStat = new StatsPerBucket();
                bucketStat.Rps = rps;

                foreach (var row in sidecarStatRows)
                {
                    if (!string.IsNullOrWhiteSpace(row))
                    {
                        var stat = row.Split(':');
                        stats.Add(stat[0], stat[1]);
                    }
                }

                bucketStat.Stats = stats;
                bucketStats.Add(bucketStat);
                Console.WriteLine();
                rps = rps + 15;
            }

            GenerateReport(bucketStats);
        }

        public static void GenerateReport(List<StatsPerBucket> bucketStats)
        {
            HashSet<string> keys = new HashSet<string>();
            List<string> finalReport = new List<string>();

            foreach (var bucket in bucketStats)
            {
                foreach (var stat in bucket.Stats)
                {
                    keys.Add(stat.Key);
                }
            }

            foreach (var key in keys)
            {
                var valuesInBuckets = key;
                foreach (var bucket in bucketStats)
                {
                    if (bucket.Stats.ContainsKey(key))
                    {
                        string value = string.Empty;
                        bucket.Stats.TryGetValue(key, out value);
                        valuesInBuckets = string.Concat(valuesInBuckets, ";", value);
                    }
                }

                finalReport.Add(valuesInBuckets);
            }
            System.IO.File.WriteAllLines(@"C:\sources\resiliencyPatterns\result.csv", finalReport.ToArray());

        }
    }

    public class StatsPerBucket
    {
        public int Rps { get; set; }
        public Dictionary<string, string> Stats { get; set; }
    }
}
