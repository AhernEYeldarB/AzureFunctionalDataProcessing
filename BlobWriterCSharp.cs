using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs;
using System;
using System.Text;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
public class BlobWriterCSharp
{
    [FunctionName("BlobWriterCSharp")]
    public static void Run([BlobTrigger("test1/{name}", Connection = "AzureWebJobsStorage")] Stream InStream, string name, ILogger log,
        [Blob("test1/{name}", FileAccess.Write)] Stream test)
    {
        byte[] byteArray;
        log.LogInformation("1");
        if (InStream != null)
        {
            log.LogInformation("2");
            byteArray = ReadAllBytes(InStream);
            string utfString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {byteArray.Length} Bytes, value: {byteArray} = {utfString}");
        }
        else
        {
            log.LogInformation("3");
            int[] intArray = { 1, 2, 3, 4, 5 };
            byte[] result = new byte[intArray.Length * sizeof(int)];
            Buffer.BlockCopy(intArray, 0, result, 0, result.Length);
        }
        log.LogInformation("4");
        return;
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

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    // Used as an ORM layer 
    public class HumanIdentity
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Root
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
// {
//   "generatedBy": "Microsoft.NET.Sdk.Functions-3.0.7",
//   "configurationSource": "attributes",
//   "bindings": [
//     {
//       "type": "orchestrationTrigger",
//       "name": "context"
//     },
//     {
//       "dataType": "stream",
//       "direction": "in",
//       "connection": "AzureWebJobsStorage",
//       "path": "test1/random-personal-info1",
//       "name": "InStream"
//     }
//   ],
//   "disabled": false,
//   "scriptFile": "../bin/csFyp.dll",
//   "entryPoint": "Company.Function.readFromBlob.RunOrchestrator"
// }