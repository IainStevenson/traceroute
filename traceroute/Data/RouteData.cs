#region Using Declarations

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.Serialization;

#endregion

namespace TraceRoute.Data
{


    /// <summary>
    /// Defines the processed IP Route data.
    /// </summary>
    [DataContract(IsReference = true)]
    [KnownType(typeof(HopData))]
    [KnownType(typeof(Data.PingData))]
    public partial class RouteData
    {
        public RouteData()
        {
            Hops = new List<HopData>();
        }
        [DataMember]
        public String Report { get; set; }
        /// <summary>
        /// The time of the route request
        /// </summary>
        [DataMember]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The Max hops allowed
        /// </summary>
        [DataMember]
        public int MaxHops { get; set; }
        /// <summary>
        /// The ping timeout used
        /// </summary>
        [DataMember]
        public int Timeout { get; set; }

        /// <summary>
        /// The route status
        /// </summary>
        [DataMember]
        public RouteState Status { get; set; }

        /// <summary>
        /// The Detination IP Address
        /// </summary>
        [IgnoreDataMember]
        public IPAddress Destination { get; set; }

        /// <summary>
        /// Is a convenience property for serializing the IPAddress
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DataMember(Name="Destination")]
        public string Address
        {
            get
            {
                // serialize
                return Destination == null ? null : Destination.ToString();
            }
            set
            {
                // deserialize
                IPAddress destination = null;
                if (IPAddress.TryParse(value, out destination)) { Destination = destination; }
            }
        }
        /// <summary>
        /// The Hop data list
        /// </summary>
        [DataMember]
        public List<HopData> Hops { get; set; }


    }
}