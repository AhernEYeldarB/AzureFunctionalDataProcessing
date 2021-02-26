using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("../readFromBlob")]


public class Pipe
{
    public string type { get; set; }
    public string callback { get; set; }
}