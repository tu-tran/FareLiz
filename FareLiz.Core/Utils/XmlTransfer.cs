using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SkyDean.FareLiz.Core.Utils
{
    /// <summary>
    /// A generic class for XML serialization
    /// </summary>
    /// <typeparam name="T">The class being supported.</typeparam>
    public class XmlTransfer<T>
    {
        #region String Operations
        /// <summary>
        /// Will Deserialize an XML String into an object.
        /// This is basically a wrapper on the XmlSerializer, and it will propagate exceptions
        /// </summary>
        /// <param name="xmlString">A string representation of an XML document</param>
        /// <returns>The deserialized object</returns>
        public static T FromXmlString(string xmlString)
        {
            T ret;
            using (StringReader sr = new StringReader(xmlString))
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));

                // We want to propagate exceptions, so no try/catch
                ret = (T)xs.Deserialize(sr);
            }
            return ret;
        }

        /// <summary>
        /// Converts this object to an string representation of an XML string
        /// This is basically a wrapper on the XmlSerializer, and it will
        /// propagate exceptions
        /// </summary>
        /// <returns>The XML string representation of this object.</returns>
        public static string ToXmlString(T instance)
        {
            if (instance == null)
                return "";

            string ret = null;
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                using (XmlWriter writer = XmlWriter.Create(sw, Constants.DefaultXmlSetting))
                {
                    // We want to propagate exceptions, so no try/catch
                    xs.Serialize(writer, instance, Constants.EmptyXmlNamespace);
                    ret = sw.ToString();
                }
            }
            return ret;
        }
        #endregion

        #region TextReaderWriter Operations
        /// <summary>
        /// Will Deserialize an stream containing XML into an object.
        /// This is basically a wrapper on the XmlSerializer, and it will
        /// propagate exceptions
        /// </summary>
        /// <param name="xmlStream">a stream containing an XML document</param>
        /// <returns>The deserialized object</returns>
        public static T FromTextReader(TextReader xmlStream)
        {
            T ret;
            XmlSerializer xs = new XmlSerializer(typeof(T));

            // We want to propagate exceptions
            ret = (T)xs.Deserialize(xmlStream);
            return ret;
        }

        /// <summary>
        /// Will serialize an object to a stream.
        /// This is basically a wrapper on the XmlSerializer, and it will
        /// propagate exceptions
        /// </summary>
        /// <param name="output">The stream to output the XML to</param>
        public static void ToTextWriter(TextWriter output, T instance)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (XmlWriter writer = XmlWriter.Create(output, Constants.DefaultXmlSetting))
            {
                xs.Serialize(writer, instance, Constants.EmptyXmlNamespace);
            }
        }
        #endregion

        #region File Operations
        /// <summary>
        /// Will deserialize an file containing XML into an object.
        /// This is basically a wrapper on the XmlSerializer, and it will
        /// propagate exceptions
        /// </summary>
        /// <param name="fileName">The file to parse</param>
        /// <returns></returns>
        public static T FromFile(string fileName)
        {
            using (StreamReader stream = new StreamReader(fileName, Encoding.Default))
                return FromTextReader(stream);
        }

        /// <summary>
        /// Will serialize an object into an XML file.
        /// This is basically a wrapper on the XmlSerializer, and it will
        /// propagate exceptions
        /// </summary>
        /// <param name="fileName"></param>
        public static void ToFile(string fileName, T instance, bool append)
        {
            using (StreamWriter stream = new StreamWriter(fileName, append, Encoding.Default))
                ToTextWriter(stream, instance);
        }
        #endregion
    }
}
