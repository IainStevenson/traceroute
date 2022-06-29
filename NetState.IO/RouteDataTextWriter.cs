using NetState.Network.Data;

namespace NetState.IO
{
    public class RouteDataTextWriter: IRequestFileWriter
    {
        public RouteDataTextWriter(DirectoryInfo path)
        {
            Path = path;
        }
        public void Write(RouteData request)
        {

            var ms = new MemoryStream( System.Text.Encoding.UTF8.GetBytes(request.Report));
            
            using (
                var file =
                    new FileStream(
                        FileNameFormatter.GetFilename(Path, request.Address, "txt"),
                        FileMode.Create))
            {
                ms.WriteTo(file);
                ms.Flush();
                file.Close();
            }
        }

        public DirectoryInfo Path { get; set; }
    }
}