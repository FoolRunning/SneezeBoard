using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SneezeBoardCommon
{
    public class UserInfo : ServerObject
    {
        [XmlText]
        public string Name;
        [XmlAttribute("userId")]
        public Guid UserGuid;
        [XmlIgnore]
        public Color Color;

        [XmlAttribute("color")]
        public string ColorStr
        {
            get { return ColorTranslator.ToHtml(Color); }
            set { Color = ColorTranslator.FromHtml(value); }
        }

        public UserInfo(string name, Guid userGuid, Color color)
        {
            Name = name;
            UserGuid = userGuid;
            Color = color;
        }

        public UserInfo()
        {

        }

        public override void DeserializeFromString(string str)
        {
            using (StringReader reader = new StringReader(str))
            {
                XmlSerializer xmlSerial = new XmlSerializer(typeof(UserInfo));
                UserInfo info = (UserInfo)xmlSerial.Deserialize(reader);
                Name = info.Name;
                UserGuid = info.UserGuid;
                Color = info.Color;
            }
        }

        public override bool Equals(object obj)
        {
            UserInfo other = obj as UserInfo;
            return other != null && other.UserGuid == UserGuid;
        }

        public override int GetHashCode()
        {
            return UserGuid.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
