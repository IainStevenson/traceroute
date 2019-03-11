using System.Diagnostics;
using TraceRoute.Data;

namespace TraceRoute.Output
{
    public class RouteDataTraceWriter : IRequestWriter
    {
        public void Write(RouteData request)
        {
            Trace.WriteLine(request.Report);
        }
    }
}