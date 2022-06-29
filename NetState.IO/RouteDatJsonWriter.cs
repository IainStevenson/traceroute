using NetState.Network.Data;
using Newtonsoft.Json;

namespace NetState.IO
{
    public class RouteJsonWriter : IRequestFileWriter
    {
        public RouteJsonWriter(DirectoryInfo path)
        {
            Path = path;
        }
        public void Write(RouteData request)
        {

            var data = JsonConvert.SerializeObject(request);
            var filename = FileNameFormatter.GetFilename(Path, request.Address, "json");
            File.WriteAllText(filename, data);

        }

        public DirectoryInfo Path { get; set; }
    }
}