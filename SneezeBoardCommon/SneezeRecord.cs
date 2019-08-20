using System;
using System.IO;
using System.Xml.Serialization;

namespace SneezeBoardCommon
{
    public class SneezeRecord : ServerObject
    {
        [XmlAttribute("userId")]
        public Guid UserId;
        [XmlAttribute("date")]
        public DateTime Date;
        [XmlText]
        public string Comment;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userGuid"></param>
        /// <param name="currentDate"></param>
        /// <param name="sneezeComment"></param>
        public SneezeRecord(Guid userGuid, DateTime currentDate, string sneezeComment = null)
        {
            UserId = userGuid;
            Date = currentDate.ToUniversalTime();
            Comment = sneezeComment;
        }

        public SneezeRecord()
        {

        }
        public override void DeserializeFromString(string str)
        {
            using (StringReader reader = new StringReader(str))
            {
                XmlSerializer xmlSerial = new XmlSerializer(typeof(SneezeRecord));
                SneezeRecord sneeze = (SneezeRecord)xmlSerial.Deserialize(reader);
                UserId = sneeze.UserId;
                Date = sneeze.Date;
                Comment = sneeze.Comment;
            }
        }
    }
}
