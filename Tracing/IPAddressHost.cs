#region Using Declarations

using System;
using System.Net;
using System.Runtime.Serialization;

#endregion

namespace TraceRoute
{
    [DataContract(IsReference = true)]
    public class IPAddressHost
    {
        private IPAddress _address;
        private string _addressAsString;

        [IgnoreDataMember]
        public IPAddress Address
        {
            get { return _address; }
            set { _address = value; }
        }

        [DataMember(Name = "Address")]
        public string AddressAsString
        {
            get { return _address.ToString(); }
            set
            {
                if (value == null) _address = null;
                else
                {
                    if (!IPAddress.TryParse(value, out _address))
                        throw new ArgumentException("The deserialised IPAddress is invalid", "AddressToString");
                }
            }
        }

        [DataMember]
        public string HostName { get; set; }
    }
}