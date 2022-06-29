using System.Diagnostics;
using NetState.Network.Data;

namespace NetState.IO
{
    public class RouteDataTraceWriter : IRequestWriter
    {
        public void Write(RouteData request)
        {
            Trace.WriteLine(request.Report);
        }
    }
}