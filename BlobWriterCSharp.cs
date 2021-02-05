using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs;
using System;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
public class BlobWriterCSharp
{
    [FunctionName("BlobWriterCSharp")]
    public static void Run([BlobTrigger("fyp1/{name}", Connection = "AzureWebJobsStorage")] Stream InStream, string name, ILogger log,
        [Blob("fyp1/{name}")] Byte[] OutArray)
    {
        byte[] byteArray;
        log.LogInformation("1");
        if (InStream != null)
        {
            log.LogInformation("2");
            byteArray = ReadAllBytes(InStream);
            for (int i = 0; i < InStream.Length; i++)
            {
                int temp = byteArray[i];
                byteArray[i] *= 2;
                log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {byteArray.Length} Bytes, value: {byteArray[i]} = {temp} x 2");
            }
            OutArray = byteArray;
        }
        else
        {
            log.LogInformation("3");
            int[] intArray = { 1, 2, 3, 4, 5 };
            byte[] result = new byte[intArray.Length * sizeof(int)];
            Buffer.BlockCopy(intArray, 0, result, 0, result.Length);
            OutArray = result;
        }
        log.LogInformation("4");
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