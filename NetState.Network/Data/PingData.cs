using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;

namespace NetState.Network.Data
{
    [DataContract(IsReference = true)]
    public partial class PingData
    {

        [IgnoreDataMember]
        public IPAddress Responder { get; set; }

        /// <summary>
        /// Is a convenience property for serializing the IPAddress
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DataMember(Name="Responder")]
        public string Address
        {
            get
            {
                // serialize
                return Responder == null ? null : Responder.ToString();
            }
            set
            {
                // deserialize
                IPAddress responder = null;
                if (IPAddress.TryParse(value, out responder)) { Responder = responder; }
            }
        }
        [DataMember]
        public DateTime Timestamp { get; set; }
        [DataMember]
        public int Attempt { get; set; }
        [DataMember]
        public long Elapsed { get; set; }
        [DataMember]
        public IPStatus Status { get; set; }

    }
}