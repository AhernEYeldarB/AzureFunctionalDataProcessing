using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
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
            BlobInfo info = context.GetInput<BlobInfo>();

            // True on completion does not mean successful
            bool test = await context.CallActivityAsync<bool>("mainActivity", info);

            return outputs;
        }

        [FunctionName("mainActivity")]
        public static bool mainActivity([ActivityTrigger] BlobInfo info,
        ILogger log,
        [Blob("test1/{info.filename}", FileAccess.Read, Connection = "AzureWebJobsStorage")] Stream InStream,
        [Blob("test1/output-{info.filename}", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream OutStream
        )
        {
            // Create Pipe and JSON blob stream
            IEnumerable<Row> humans = JsonReaderExtensions.convertToJsonIterable(InStream);
            // IEnumerable<Pipe> pipeline = JsonReaderExtensions.convertToJsonIterable(info.pipeline);


            Func<Row, object> mapFunction = value =>
            {
                var obj = new
                {
                    name = value.name,
                    eyeColor = value.eyeColor
                };
                return obj;
            };

            Func<dynamic, bool> filterPredicate = value =>
            {
                return value.eyeColor == "green";
            };

            Func<IEnumerable, IEnumerable> pipeline = Activities.pipelineMaker(
                Activities.mapMaker<Row, dynamic>(mapFunction),
                Activities.eachMaker(),
                Activities.filterMaker(filterPredicate)
            );

            // Prepare Output stream writer
            using (JsonTextWriter wr = JsonReaderExtensions.InitJsonOutStream(OutStream))
            {
                wr.WriteStartArray();
                foreach (var h in pipeline(humans))
                {
                    wr.SerialiseJsonToStream<dynamic>(h);
                }
                wr.WriteEndArray();
            }
            return true;
        }

        [FunctionName("printObjectPropertiesSensor")]
        public static Row printObjectPropertiesSensor([ActivityTrigger] Row h, ILogger log)
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
    }
}