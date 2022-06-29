#region Using Declarations

using TraceRoute.Data;

#endregion

namespace TraceRoute
{
    public interface IRequestWriter
    {
        
        void Write(RouteData request);
    }
}