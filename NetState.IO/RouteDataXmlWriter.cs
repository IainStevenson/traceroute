using System.Runtime.Serialization;
using System.Xml;
using NetState.Network.Data;

namespace NetState.IO
{
    public class RouteDataXmlWriter : IRequestFileWriter
    {
         public RouteDataXmlWriter(DirectoryInfo path)
        {
            Path = path;
        }
        public void Write(RouteData request)
        {
            var ds = new DataContractSerializer(typeof (RouteData));
            var ms = new MemoryStream();
            var settings = new XmlWriterSettings {Indent = true};
            using (var w = XmlWriter.Create(ms, settings))
            {
                ds.WriteObject(w, request);
            }

            using (
                var file =
                    new FileStream(
                       FileNameFormatter.GetFilename(Path, request.Address, "xml"),
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