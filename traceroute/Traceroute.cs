#region Using Declarations

using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using TraceRoute.Data;

#endregion

namespace TraceRoute
{
    public class TraceRoute
    {
        /// <summary>
        ///     Address name resolvers for IP Addresses. A DNS function substitute for unresolvable addresses.
        /// </summary>
        private readonly IPAddressResolver _resolvers;

        public TraceRoute(IPAddressResolver resolvers)
        {
            _resolvers = resolvers;
        }

        /// <summary>
        ///     Processes a Trace request
        /// </summary>
        /// <param name="ipAddress">The detsination IP address</param>
        /// <param name="hop1Address">The first Hop Address as it allways returns null</param>
        /// <param name="maxHops">The max hops to try.</param>
        /// <param name="timeout">The ping timeout.</param>
        /// <returns>
        ///     A new instance of <see cref="Data.RouteData" /> holding the trace results.
        /// </returns>
        public RouteData ProcessRequest(IPAddress ipAddress,
                                        IPAddress hop1Address,
                                        int maxHops = 30,
                                        int timeout = 5000)
        {
            var result = new RouteData
                {
                    Destination = ipAddress,
                    MaxHops = maxHops,
                    Timeout = timeout,
                    Timestamp = DateTime.Now
                };

            const int maxAttempts = 3;

            using (var pingSender = new Ping())
            {
                var pingOptions = new PingOptions();
                var stopWatch = new Stopwatch();
                var bytes = new byte[32];

                pingOptions.DontFragment = true;
                pingOptions.Ttl = 1; // start hopping from 1 to maxHops

                for (var i = 1; i < maxHops + 1; i++)
                {
                    var hop = new HopData {Hop = i, Previous = i == 1 ? null : result.Hops[result.Hops.Count - 1]};

                    PingReply pingReply = null;
                    IPAddress hopAddress = null;
                    for (var attempt = 1; attempt <= maxAttempts; attempt++)
                    {

                        try
                        {
                            stopWatch.Restart();
                            pingReply = pingSender.Send(ipAddress, timeout, bytes, pingOptions);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(String.Format("TraceRoute: {0}",ex.Message));
                        }
                        finally
                        {
                            stopWatch.Stop();
                        }


                        if (pingReply == null) continue;
                        if (attempt == 1) hopAddress = pingReply.Address;
                        var pingResult = new PingData
                            {
                                Status = pingReply.Status,
                                Responder = hopAddress,
                                Attempt = attempt,
                                Elapsed = stopWatch.ElapsedMilliseconds,
                                Timestamp = DateTime.Now
                            };
                        hop.Pings.Add(pingResult);
                    }
                    
                    var ipHostName = "unknown";

                    if (pingReply != null && (i == 1 && pingReply.Address == null))
                    {
                        hopAddress = hop1Address;
                    }

                    if (hopAddress != null)
                    {
                        ipHostName = result.GetHostName(hopAddress);
                    }

                    if (ipHostName == String.Empty)
                    {
                        // see if we have a resolver for this address otherwise leave it blank.
                        var resolvedAddress = _resolvers.Resolve(hopAddress);
                        if (resolvedAddress != null)
                        {
                            ipHostName = resolvedAddress;
                        }
                    }

                    //var address = pingReply.Address == null ? "" : pingReply.Address.ToString();


                    hop.HostName = ipHostName;
                    if (pingReply != null) hop.Status = pingReply.Status;

                    hop.Responder = hopAddress;
                    result.Hops.Add(hop);

                    if (pingReply != null && pingReply.Status == IPStatus.Success)
                    {
                        break;
                    }

                    pingOptions.Ttl++;
                }
            }
            result.Report = result.ToString();
            result.Status = result.Hops[result.Hops.Count - 1].Status == IPStatus.Success
                                ? RouteState.Working
                                : RouteState.Failed;
            return result;
        }
    }
}