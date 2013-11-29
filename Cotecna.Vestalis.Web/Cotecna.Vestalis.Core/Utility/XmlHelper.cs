using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Cotecna.Vestalis.Core
{
    public static class XmlHelper
    {
        public static T ReadFormFromXml<T>(string xmlDefinition)
        {
            return ReadObjectFromXmlString<T>(xmlDefinition);
        }

        /// <summary>
        /// Returns the object represented in the xml string
        /// </summary>
        /// <param name="xml">The XML string</param>
        /// <returns></returns>
        public static T ReadObjectFromXmlString<T>(string xml)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                stream = new StringReader(xml);
                reader = new XmlTextReader(stream);
                return (T)serializer.Deserialize(reader);
            }
            catch
            {
                return default(T);
            }
            finally
            {
                if (stream != null) stream.Dispose();
            }
        }
    }
}
