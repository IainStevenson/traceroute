using System;
using System.IO;
using TraceRoute.Data;

namespace TraceRoute.Output
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
                        string.Format(@"{0}\Route-{1}.txt", Path.FullName, string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now)),
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