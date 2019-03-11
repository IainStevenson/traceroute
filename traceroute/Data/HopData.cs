#region Using Declarations

using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;

#endregion

namespace TraceRoute.Data
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(IsReference = true)]
    [KnownType(typeof(PingData))]
    public partial class HopData
    {
        public const string TimeoutMessage = "Request timed out.";
        public HopData()
        {
            Pings = new List<PingData>();
        }

        [DataMember]
        public HopData Previous { get; set; }
        [DataMember]
        public int Hop { get; set; }

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
        public List<PingData> Pings { get; set; }
        [DataMember]
        public IPStatus Status { get; set; }
        [DataMember]
        public string HostName { get; set; }
        
    }



}