using System;
using TraceRoute.Data;

namespace TraceRoute.Output
{
    public class RouteDataConsoleWriter : IRequestWriter
    {
        public void Write(RouteData request)
        {
            Console.WriteLine("{0}", request);
        }


    }
}