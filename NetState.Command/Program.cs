using NetState.Network;
using NetState.IO;
using Newtonsoft.Json;

namespace NetState.Command
{

    internal class Program
    {
        private static List<IRequestWriter> _handlers = new List<IRequestWriter>();

        private static void Main(string[] args)
        {
            var settings = new Settings();
            settings = JsonConvert.DeserializeObject<Settings>(System.IO.File.ReadAllText(".\\appSettings.json"));

            var xmlFolder = new DirectoryInfo(settings.XmlDataPath);
            var textFolder = new DirectoryInfo(settings.TextDataPath);
            var jsonFolder = new DirectoryInfo(settings.JsonDataPath);

            var ipDefaultGateway = TraceRoute.GetDefaultGateway();
            if (ipDefaultGateway != null)
            {
                var resolver = TraceRoute.GetLocalAddressResolver(settings.LocalHostsFile);
                var destinations = args.ToList();
                var targets = new IPAddresses { Addresses = destinations };
                _handlers.Add(new RouteDataTraceWriter());
                _handlers.Add(new RouteDataConsoleWriter());
                _handlers.Add(new RouteDataXmlWriter(xmlFolder));
                _handlers.Add(new RouteDataTextWriter(textFolder));
                _handlers.Add(new RouteJsonWriter(jsonFolder));
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
                                    var ipHostEntry = System.Net.Dns.GetHostEntry(address);




                                    var ipAddress = ipHostEntry.AddressList[0];

                                    var tracer = new TraceRoute(resolver);
                                    var request = tracer.ProcessRequest(ipAddress, ipDefaultGateway,
                                                                         settings.MaxHops,
                                                                         settings.Timeout);
                                    foreach (var handler in _handlers)
                                    {
                                        handler.Write(request);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                                    throw;
                                }

                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Trace.WriteLine(e.Message);
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

        

    }
}