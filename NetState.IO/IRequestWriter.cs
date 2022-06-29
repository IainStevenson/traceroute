#region Using Declarations

using NetState.Network.Data;

#endregion

namespace NetState.IO
{
    public interface IRequestWriter
    {
        
        void Write(RouteData request);
    }
}