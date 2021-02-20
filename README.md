# Working with Azure Functions
# 1. Install and Start the Azurite Local Emulator
https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite
(Installed with npm and run in command line)

### CMD command (Preffered)
In windows CMD you can run ```START start-azurite.cmd```
or directly run ```azurite --silent --location .\azurite --debug .\azurite\debug.log```

### Install and use vscode extension and manage from within VSCode

# 2. Open Azure Storage Explorer
<p>
This tool is used to  monitor the state of the Azurite Local Storage Emulator.
Using this tool files can be uploaded and passed to the blob trigger.
</p>


# 3. Find or Generate your JSON data to Process in the Pipeline

<p> Random Json Objects can be generated here <a href="https://www.json-generator.com/#"> </p>

# 4. Generate a C\# class that will act as a model for the dataset
<p>
Pass the JSON data above through here to generate the model to avoid having to constuct the class manually.
<a href="https://json2csharp.com/">

Add this class to your blob trigger C# file.
</p>


curl http://localhost:7071/api/readFromBlob1_HttpStart -d "{"filename": "random-personal-info1.json"}"

# 5. Create your pipeline