#region Using Declarations

using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

#endregion

namespace TraceRoute.Extensions
{
    /// <summary>
    ///     Provides extensions for De/Serialisation of objects to and from Streams.
    /// </summary>
    public static class SerlializationExtensions
    {
        #region Serializable class methods

        /// <summary>
        ///     Serializes the specified item into a MemoryStream. Use for classes which are not declared with a DataContract attribute.
        /// </summary>
        /// <typeparam name="T"> The type of object to serialize </typeparam>
        /// <param name="item"> The item data. </param>
        /// <returns>
        ///     An instance of a <see cref="MemoryStream" /> positioned at its start, containing the objects data as serialized Xml
        /// </returns>
        public static Stream ToStream<T>(this T item) where T : class
        {
            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof (T));
            serializer.Serialize(stream, item);
            stream.GotoStart();
            return stream;
        }

        /// <summary>
        ///     Deserializes a Stream to its requested object Type
        /// </summary>
        /// <typeparam name="T"> The type of object that is contained in the stream </typeparam>
        /// <param name="stream"> The stream containing the objet to deserialize </param>
        /// <returns>
        ///     An instance of type <see cref="T" /> containing the deserialized stream data
        /// </returns>
        public static T ToType<T>(this Stream stream) where T : class
        {
            stream.GotoStart();
            //var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
            var ser = new XmlSerializer(typeof (T));
            var result = (T) ser.Deserialize(stream);
            //reader.Close();
            return result;
        }

        #endregion

        #region IExtensibleDataObject searialization methods

        /// <summary>
        ///     Searialises an Extensible DataContract object to a Stream
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="source"> </param>
        /// <returns> </returns>
        public static Stream DataContractToStream<T>(this T source) where T : IExtensibleDataObject
        {
            var stream = new MemoryStream();
            var ser = new DataContractSerializer(typeof (T));
            ser.WriteObject(stream, source);
            stream.GotoStart();
            return stream;
        }

        /// <summary>
        ///     Deserializes a Stream using an Extensible DataContract serializer
        /// </summary>
        /// <typeparam name="T"> The type of object that is contained in the stream </typeparam>
        /// <param name="stream"> The stream containing the objet to deserialize </param>
        /// <returns>
        ///     An instance of type <see cref="T" /> containing the deserialized stream data
        /// </returns>
        public static T StreamToDataContract<T>(this Stream stream) where T : IExtensibleDataObject
        {
            stream.GotoStart();
            var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
            var ser = new DataContractSerializer(typeof (T));
            var result = (T) ser.ReadObject(reader, true);
            reader.Close();
            return result;
        }

        #endregion
    }
}