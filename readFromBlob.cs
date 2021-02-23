using System.Collections.Generic;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class readFromBlob
    {
        [FunctionName("beginPipeline")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            [Blob("test1/random-personal-info1.json", FileAccess.Read, Connection = "AzureWebJobsStorage")] Stream InStream,
            ILogger log
            )
        {
            var outputs = new List<string>();

            IEnumerable<Human> humans = getHumanIterable(InStream);

            // Begin pipeline
            foreach (Human h in humans)
            {
                Human tempH = await context.CallActivityAsync<Human>("printObjectPropertiesSensor", h);
                log.LogInformation($"{h.greeting}");
            }

            return outputs;
        }

        [FunctionName("printObjectPropertiesSensor")]
        public static Human printHumanIdentityClassa([ActivityTrigger] Human h, ILogger log)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(h))
            {
                string n = descriptor.Name;
                object value = descriptor.GetValue(h);
                log.LogInformation($"${n}=${value}");
            }
            return h;
        }

        [FunctionName("HttpPipelineTrigger")]
        public static async Task HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] BlobInfo info,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("beginPipeline", info); // Calling orchestrator
            log.LogInformation($"Started orchestration with ID = '${instanceId}'.");

            // return starter.CreateCheckStatusResponse(req, instanceId);
        }
        public static IEnumerable<Human> getHumanIterable(Stream s)
        {
            // Create JSON generator
            var regex = new Regex(@"^\[\d+\]$");
            IEnumerable<Human> iterableHumans;
            StreamReader sr = new StreamReader(s);
            JsonReader reader = new JsonTextReader(sr);
            iterableHumans = reader.SelectTokensWithRegex<Human>(regex);
            return iterableHumans;
        }
    }

    public class BlobInfo
    {
        public string filename { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    // Used as an ORM layer 
    public class HumanIdentity
    {
        public List<Human> data { get; set; }
    }

    public class Human
    {
        public string _id { get; set; }
        public int index { get; set; }
        public string guid { get; set; }
        public bool isActive { get; set; }
        public string balance { get; set; }
        public string picture { get; set; }
        public int age { get; set; }
        public string eyeColor { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string company { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string about { get; set; }
        public DateTime registered { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public List<string> tags { get; set; }
        public List<HumanIdentity> HumanIdentities { get; set; }
        public string greeting { get; set; }
        public string favoriteFruit { get; set; }
    }
    // https://stackoverflow.com/questions/43747477/how-to-parse-huge-json-file-as-stream-in-json-net
    public static class JsonReaderExtensions
    {
        // Returns an iterable that yields JSON objects from the structure given in the regex. In this case [ { obj1 }, { obj2 } ]
        public static IEnumerable<T> SelectTokensWithRegex<T>(
            this JsonReader jsonReader, Regex regex)
        {
            JsonSerializer serializer = new JsonSerializer();
            while (jsonReader.Read())
            {
                if (regex.IsMatch(jsonReader.Path)
                    && jsonReader.TokenType != JsonToken.PropertyName)
                {
                    yield return serializer.Deserialize<T>(jsonReader);
                }
            }
        }
    }
}
