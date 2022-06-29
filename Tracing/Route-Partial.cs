#region Using Declarations

using System;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

#endregion

namespace TraceRoute.Data
{
    public partial class PingData
    {
        public override string ToString()
        {
            return string.Format("{0,2} {1,4} {2} {3} {4}", Attempt, Elapsed, Responder, Timestamp, Status);
        }

    }

    public partial class HopData
    {
        public override string ToString()
        {
            return string.Format("{0,2} {1} {5} {2} {5} {3} {5} {4}\r\n",
                                 Hop,
                                 PrepareTime(0),
                                 PrepareTime(1),
                                 PrepareTime(2),
                                 Status == IPStatus.TimedOut
                                     ? TimeoutMessage
                                     : HostName == String.Empty
                                           ? Responder.ToString()
                                           : String.Format("{0} [{1}]", HostName, Responder),
                                 Status == IPStatus.TimedOut
                                     ? "  "
                                     : "ms"
                );
        }

        private string PrepareTime(int pingInstance)
        {
            if (Status == IPStatus.TimedOut) return "*".PadLeft(4);
            var t1 = Pings[pingInstance].Elapsed <= 1
                         ? "<1".PadLeft(4)
                         : Pings[pingInstance].Elapsed.ToString(CultureInfo.InvariantCulture).PadLeft(4);
            return t1;
        }
    }

    public partial class RouteData
    {
        public override string ToString()
        {
            var sb =
                new StringBuilder(String.Format("Tracing route to {0} [{1}] \r\nover a maximum of {2} hops: {3:yyyy-MM-dd hh:mm:ss}\r\n",
                                                GetHostName(Destination), Destination, MaxHops, DateTime.Now));
            foreach (var hop in Hops)
            {
                sb.Append((object) hop);
            }
            sb.Append("\r\nTrace Complete.\r\n");
            return sb.ToString();
        }

        public string GetHostName(IPAddress address)
        {
            if (address != null)
            {
                try
                {
                    var ipHostName = Dns.GetHostEntry(address).HostName;
                    return ipHostName;
                }
                catch (SocketException)
                {
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return "";
        }
    }
}