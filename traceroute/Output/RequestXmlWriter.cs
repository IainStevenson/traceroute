using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using TraceRoute.Data;

namespace TraceRoute.Output
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
                        string.Format(@"{0}\Route-{1}.xml", Path.FullName, string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now)),
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