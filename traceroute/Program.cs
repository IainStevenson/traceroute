#region Using Declarations

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Xml;
using ConsoleApplication2;
using TraceRoute.Output;
using TraceRoute.Properties;

#endregion

namespace TraceRoute
{
    internal class Program
    {

        private static List<IRequestWriter> _handlers = new List<IRequestWriter>();

        private static void Main(string[] args)
        {
            var ipDefaultGateway = GetDefaultGateway();
            if (ipDefaultGateway != null)
            {
                var resolver = GetLocalAddressResolver();
                var destinations = args.ToList();
                var targets = new IPAddresses { Addresses = destinations };
                _handlers.Add(new RouteDataTraceWriter());
                _handlers.Add(new RouteDataConsoleWriter());
                _handlers.Add(new RouteDataXmlWriter(new DirectoryInfo(Settings.Default.XmlDataPath)));
                _handlers.Add(new RouteDataTextWriter(new DirectoryInfo(Settings.Default.TextDataPath)));
                Console.Title = "TraceRoute - Press escape key to stop";
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        try
                        {
                            
                            foreach (var address in targets.Addresses)
                            {

                                try
                                {
                                    var ipHostEntry = Dns.GetHostEntry(address);
                                    



                                    var ipAddress = ipHostEntry.AddressList[0];

                                    var tracer = new TraceRoute(resolver);
                                    var request = tracer.ProcessRequest(ipAddress, ipDefaultGateway,
                                                                         Settings.Default.MaxHops,
                                                                         Settings.Default.Timeout);
                                    foreach(var handler in _handlers)
                                    {
                                        handler.Write(request);
                                    }
                                }
                                catch (Exception)
                                {

                                    throw;
                                }
                            
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                        }
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }
            else
            {
                Console.WriteLine("The default gateway could not be found, please ensure that the network is operational and try again.");
                Console.ReadKey(true);
            }
        }

        private static IPAddressResolver GetLocalAddressResolver()
        {
            var resolver = new IPAddressResolver();
            var fileName = Settings.Default.LocalHostsFile;
            if (File.Exists(fileName))
            {
                var s = new DataContractSerializer(typeof(IPAddressResolver));
                using (var f = new StreamReader(fileName))
                {
                    resolver = (IPAddressResolver)s.ReadObject(f.BaseStream);
                }
            }
            else
            {
                resolver.Addresses.Add(new IPAddressHost
                {
                    Address = IPAddress.Parse("192.168.0.1"),
                    HostName = "Default Gateway"
                });
                resolver.Addresses.Add(new IPAddressHost
                {
                    Address = IPAddress.Parse("192.168.1.1"),
                    HostName = "Router -> EE Wan 1"
                });
                resolver.Addresses.Add(new IPAddressHost
                {
                    Address = IPAddress.Parse("172.16.0.4"),
                    HostName = "Router -> EE Wan 2 "
                });                
            }
            var ds = new DataContractSerializer(typeof(IPAddressResolver));
            var ms = new MemoryStream();
            var settings = new XmlWriterSettings { Indent = true };
            using (var w = XmlWriter.Create(ms, settings))
            {
                ds.WriteObject(w, resolver);
            }

            using (var file = new FileStream(string.Format(@"{0}", fileName), FileMode.Create))
            {
                ms.WriteTo(file);
                ms.Flush();
                file.Close();
            }


            return resolver;
        }

        private static IPAddress GetDefaultGateway()
        {
            IPAddress ipDefaultGateway = null;

            var allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                                                       .Where(
                                                           x =>
                                                           (x.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                                                            x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                                                           && x.OperationalStatus == OperationalStatus.Up)
                                                       .OrderBy(y => y.NetworkInterfaceType)
                                                       .ThenBy(z => z.Name);
            var card = allNetworkInterfaces.FirstOrDefault();
            if (card != null)
            {
                var ipInterfaceProperties = card.GetIPProperties();
                var gatewayIpAddressInformationCollection = ipInterfaceProperties.GatewayAddresses;
                var address = gatewayIpAddressInformationCollection.FirstOrDefault();
                if (address != null) ipDefaultGateway = address.Address;
            }
            return ipDefaultGateway;
        }
    }
}