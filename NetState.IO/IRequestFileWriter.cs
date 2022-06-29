namespace NetState.IO
{
    public interface IRequestFileWriter : IRequestWriter
    {
        DirectoryInfo Path { get; set; }
    }
}