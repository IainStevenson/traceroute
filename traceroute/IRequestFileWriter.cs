namespace TraceRoute
{
    public interface IRequestFileWriter : IRequestWriter
    {
        System.IO.DirectoryInfo Path { get; set; }
    }
}