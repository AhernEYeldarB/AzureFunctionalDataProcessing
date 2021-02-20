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
            ILogger log
            )
        {
            var outputs = new List<string>();

            Human h = context.GetInput<Human>();

            if (h != null)
            {
                await context.CallActivityAsync<Human>("printParsedJson", h);
            }
            // outputs.Add(await context.CallActivityAsync<string>("parseBlobJson", "Seattle"));
            // outputs.Add(await context.CallActivityAsync<string>("parseBlobJson", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("printParsedJson")]
        public static void printHumanIdentityClassa([ActivityTrigger] Human h, ILogger log)
        {
            log.LogInformation($"\n**************\n${h.greeting}\n");
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(h))
            {
                string n = descriptor.Name;
                object value = descriptor.GetValue(h);
                log.LogInformation($"${n}=${value}");
            }
        }

        [FunctionName("readFromBlob1_HttpStart")]
        public static async Task HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] BlobInfo info,
            [Blob("test1/{filename}", FileAccess.Read, Connection = "AzureWebJobsStorage")] Stream InStream,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            log.LogInformation($"C# Blob trigger function Processed blob name: ${info}");
            if (InStream != null)
            {
                byte[] streamAsByteArray = ReadAllBytes(InStream);
                string parsedStream = Regex.Replace(Encoding.UTF8.GetString(streamAsByteArray, 0, streamAsByteArray.Length), @"\s+", "");
                log.LogInformation($"C# Blob trigger function Processed blob\n Name:{info} \n Size: {streamAsByteArray.Length} Bytes, value: {streamAsByteArray} = {parsedStream}");
                HumanIdentity humans = JsonConvert.DeserializeObject<HumanIdentity>(parsedStream);
                foreach (Human h in humans.data)
                {
                    string instanceId = await starter.StartNewAsync("beginPipeline", h); // Calling orchestrator
                    log.LogInformation($"Started orchestration with ID = '${instanceId}'.");
                }

            }

            // return starter.CreateCheckStatusResponse(req, instanceId);
        }

        public static byte[] ReadAllBytes(Stream instream)
        {
            if (instream is MemoryStream)
                return ((MemoryStream)instream).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                instream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
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
}