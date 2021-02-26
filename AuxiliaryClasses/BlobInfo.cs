using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("../readFromBlob")]

public class BlobInfo
{
    public string filename { get; set; }
    public string pipeline { get; set; }
}