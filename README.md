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

<p>
    <a href="https://www.json-generator.com/#"> You can generate random JSON obejcts here</a>
</p>

# 4. Generate a C\# class that will act as a model for the dataset
<p>
    <a href="https://json2csharp.com/">
    Pass the JSON data above through here to generate the model to avoid having to constuct the class manually.
    </a>
    Add this class to your blob trigger C# file.
</p>

# 5. Create your pipeline
You will need:
1. An `IEnumerable` type to be the input to the pipeline
2. A set of pipeline activites that are to be chained together using `pipelineMaker` function.
    * This function returns a function which is where you pass in `IEnumerable` your input

For example, a pipeline that take a foreach with no callback (Essentially a passthrough that does nothing) and a filter that returns the rows where "row = {music : 'loud'}"
```c#
    // Some incoming object that is IEnumerable
    IEnumerable InStream;

    Func<Row, bool> filterPredicate = value =>
    {
        return value.music == "loud";
    };

    Func<IEnumerable, IEnumerable> pipeline = Activities.pipelineMaker(
        Activities.eachMaker(),
        Activities.filterMaker(filterPredicate)
    );

    foreach (Row h in pipeline(InStream))
    {
        // Do something with resulting row
    }
```

# 6. Call your function once uploaded using a post request with the blob filename and pipeline WIP
* Bash 
```bash
curl http://localhost:7071/api/HttpPipelineTrigger -d "{'filename': 'random-personal-info1.json','pipeline': '[]'}"
```
* Powershell
```PowerShell
Invoke-RestMethod -Method "Post" -URI "http://localhost:7071/api/HttpPipelineTrigger" -Body "{filename : 'random-personal-info1.json', pipeline : '[]'}"
```