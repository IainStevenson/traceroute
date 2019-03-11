#region Using Declarations

using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

#endregion

namespace TraceRoute.Extensions
{
    /// <summary>
    ///     Stream extensions to aid in stream managment and the conversion of streams to and from strings
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        ///     Deserializes a Stream using an Extensible DataContract serializer.
        /// </summary>
        /// <typeparam name="T"> The type of object that is contained in the stream. </typeparam>
        /// <param name="source"> The stream containing the object to deserialize. </param>
        /// <returns>
        ///     An instance of type <see cref="T" /> containing the deserialized stream data.
        /// </returns>
        public static T AsDataContractType<T>(this Stream source) where T : IExtensibleDataObject
        {
            source.GotoStart();
            var reader = XmlDictionaryReader.CreateTextReader(source, new XmlDictionaryReaderQuotas());
            var ser = new DataContractSerializer(typeof (T));
            var result = (T) ser.ReadObject(reader, true);
            reader.Close();
            return result;
        }

        /// <summary>
        ///     Serializes the specified item into a MemoryStream. Use for classes which are not declared with a DataContract attribute.
        /// </summary>
        /// <typeparam name="T"> The type of object to serialize. </typeparam>
        /// <param name="source"> The item data. </param>
        /// <returns>
        ///     An instance of a <see cref="MemoryStream" /> positioned at its start, containing the objects data as serialized Xml.
        /// </returns>
        public static Stream SerializeToStream<T>(this T source) where T : class
        {
            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof (T));
            serializer.Serialize(stream, source);
            stream.GotoStart();
            return stream;
        }

        /// <summary>
        ///     Searialises an Extensible DataContract object to a Stream
        /// </summary>
        /// <typeparam name="T"> The Type of the source data</typeparam>
        /// <param name="source"> The source data </param>
        /// <returns>
        ///     An instance of a <see cref="MemoryStream" /> positioned at its start, containing the objects data as serialized Xml.
        /// </returns>
        public static Stream DataContractAsStream<T>(this T source) where T : IExtensibleDataObject
        {
            var stream = new MemoryStream();
            var ser = new DataContractSerializer(typeof (T));
            ser.WriteObject(stream, source);
            stream.GotoStart();
            return stream;
        }

        /// <summary>
        ///     Returns a stream as a byte array.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <returns>The stream as a byte array. </returns>
        /// <remarks>The source must be readable, if not an empty byte array is returned.</remarks>
        public static byte[] AsByteArray(this Stream source)
        {
            var result = new byte[] {};

            if (source.CanRead)
            {
                result = new byte[source.Length];
                source.Read(result, 0, (int) source.Length);
                source.Close();
            }
            return result;
        }


        /// <summary>
        ///     Attempts to reposition a stream at its start.
        ///     Provides Fluent Linke interface
        /// </summary>
        /// <param name="source"> The stream to position </param>
        public static Stream GotoStart(this Stream source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (!source.CanSeek) throw new InvalidOperationException("Cannot seek on stream");
            if (source.Position != 0)
            {
                source.Seek(0, SeekOrigin.Begin);
            }
            return source;
        }

        /// <summary>
        ///     Attempts to reposition a stream at its end.
        ///     Provides Fluent Linke interface
        /// </summary>
        /// <param name="source"> the stream to position </param>
        public static Stream GotoEnd(this Stream source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (!source.CanSeek) throw new InvalidOperationException("Cannot seek on stream");
            if (source.Position != source.Length)
            {
                source.Seek(0, SeekOrigin.End);
            }
            return source;
        }

        /// <summary>
        ///     Renders a stream into a string using the supplied Encoding or its default of UTF8
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static String AsString(this Stream source, Encoding encoding = null)
        {
            if (source == null) throw new ArgumentNullException("source");
            var encodingToUse = encoding ?? Encoding.UTF8;
            source.GotoStart();
            var reader = new StreamReader(source, encodingToUse);
            var text = reader.ReadToEnd();
            return text;
        }


        /// <summary>
        ///     Converts a string into a MemoryStream using the supplied Encoding or its default of Default is UTF8, and repositions it to its start
        /// </summary>
        /// <param name="source"> The source string to convert </param>
        /// <param name="encoding"> Specifies the encoding to use. The default encoding is UTF8 </param>
        /// <returns>
        ///     A <see cref="MemoryStream" /> containing the string positioned at its start
        /// </returns>
        public static Stream AsStream(this string source, Encoding encoding = null)
        {
            if (source == null) throw new ArgumentNullException("source");
            var encodingToUse = encoding ?? Encoding.UTF8;
            var byteArray = encodingToUse.GetBytes(source);
            var stream = new MemoryStream(byteArray);
            stream.GotoStart();
            return stream;
        }


        /// <summary>
        ///     Searialises a DataContract or plain Serializable object to a new Stream
        /// </summary>
        /// <typeparam name="T"> The type of object that is contained in the stream </typeparam>
        /// <param name="source">The instance of the type to serialize </param>
        /// <returns> </returns>
        public static Stream Serialize<T>(this T source) where T : class
        {
            if (source == null) throw new ArgumentNullException("source");
            var stream = new MemoryStream();
            if (typeof (T).GetCustomAttributes(false).Any(x => x is DataContractAttribute))
            {
                var ser = new DataContractSerializer(typeof (T));
                ser.WriteObject(stream, source);
            }
            else
            {
                var xmSer = new XmlSerializer(typeof (T));
                xmSer.Serialize(stream, source);
            }
            stream.GotoStart();
            return stream;
        }


        /// <summary>
        ///     Searialises a DataContract or plain Serializable object to a new Stream
        /// </summary>
        /// <typeparam name="T"> The type of object that is contained in the stream </typeparam>
        /// <param name="source">The instance of the type to serialize </param>
        /// <param name="types"></param>
        /// <returns> </returns>
        public static Stream Serialize<T>(this T source, Type[] types) where T : class
        {
            if (source == null) throw new ArgumentNullException("source");
            var stream = new MemoryStream();
            if (typeof (T).GetCustomAttributes(false).Any(x => x is DataContractAttribute))
            {
                var ser = new DataContractSerializer(typeof (T), types);
                ser.WriteObject(stream, source);
            }
            else
            {
                var xmSer = new XmlSerializer(typeof (T), types);
                xmSer.Serialize(stream, source);
            }
            stream.GotoStart();
            return stream;
        }

        /// <summary>
        ///     Deserializes a Stream to its specified Type using the DataContract serializer (if the type is decorated with a DataContract), or the XmlSerializer
        /// </summary>
        /// <typeparam name="T"> The type of object that is contained in the stream </typeparam>
        /// <param name="source"> The stream containing the objet to deserialize </param>
        /// <returns>
        ///     An instance of type <see cref="T" /> containing the deserialized stream data
        /// </returns>
        public static T Deserialize<T>(this Stream source) where T : class
        {
            if (source == null) throw new ArgumentNullException("source");

            source.GotoStart();
            if (typeof (T).GetCustomAttributes(false).Any(x => x is DataContractAttribute))
            {
                using (
                    var reader = XmlDictionaryReader.CreateTextReader(source,
                                                                      new XmlDictionaryReaderQuotas()))
                {
                    var ser = new DataContractSerializer(typeof (T));
                    var result = (T) ser.ReadObject(reader, true);
                    reader.Close();
                    source.Close();
                    return result;
                }
            }
            var xmlSer = new XmlSerializer(typeof (T));
            var item = (T) xmlSer.Deserialize(source);
            source.Close();
            return item;
        }
    }
}