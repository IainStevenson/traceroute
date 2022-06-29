using NetState.Network.Data;

namespace NetState.IO
{
    public class RouteDataConsoleWriter : IRequestWriter
    {
        public void Write(RouteData request)
        {
            Console.WriteLine("{0}", request);
        }


    }
}