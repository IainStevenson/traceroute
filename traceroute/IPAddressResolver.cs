#region Using Declarations

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

#endregion

namespace TraceRoute
{
    [DataContract(IsReference = true)]
    public class IPAddressResolver
    {
        public IPAddressResolver()
        {
            Addresses = new List<IPAddressHost>();
        }

        [DataMember]
        public List<IPAddressHost> Addresses { get; set; }


        public string Resolve(IPAddress address)
        {
            var item = Addresses.FirstOrDefault(x=> Equals(x.Address, address));
            if (item == null) return string.Empty;

            return item.HostName;
        }
    }
}