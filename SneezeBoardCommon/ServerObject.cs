using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SneezeBoardCommon
{
    public abstract class ServerObject
    {
        public abstract void DeserializeFromString(string str);

        public string SerializeToString()
        {
            using (StringWriter writer = new StringWriter())
            {
                SerializeToStream(writer);
                return writer.ToString();
            }
        }
        
        protected void SerializeToStream(TextWriter textWriter)
        {
            XmlSerializer xmlSerial = new XmlSerializer(GetType());
            XmlSerializerNamespaces emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(textWriter, settings))
            {
                xmlSerial.Serialize(writer, this, emptyNamespaces);
            }
        }
    }
}
