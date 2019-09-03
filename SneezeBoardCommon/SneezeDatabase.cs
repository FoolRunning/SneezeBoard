using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SneezeBoardCommon
{
    public class SneezeDatabase : ServerObject
    {
        private const string fileNameFormat = "database{0}.xml";
        private const int numOfBackups = 4;
        private static string DBDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SneezeBoard");
        private static string DBPath => Path.Combine(DBDir, "database.xml");

        public int CountdownStart = 27002;

        [XmlAttribute]
        public int Version;

        [XmlArray("Sneezes")]
        [XmlArrayItem("Sneeze")]
        public List<SneezeRecord> Sneezes = new List<SneezeRecord>();

        [XmlArray("Users")]
        [XmlArrayItem("User")]
        public UserInfo[] Users
        {
            get { return IdToUser.Values.ToArray(); }
            set { IdToUser = value.ToDictionary(ui => ui.UserGuid); }
        }

        [XmlIgnore]
        public Dictionary<Guid, UserInfo> IdToUser = new Dictionary<Guid, UserInfo>();

        public bool Load()
        {
            IReadOnlyList<FileInfo> existingFiles = GetDBSaveFiles();
            if(existingFiles.Count == 0)
                return false;

            using (TextReader reader = new StreamReader(existingFiles[existingFiles.Count - 1].FullName))
                DeserializeFromStream(reader);
            return true;
        }

        public void Save()
        {
            if (!Directory.Exists(DBDir))
                Directory.CreateDirectory(DBDir);

            IReadOnlyList<FileInfo> existingFiles = GetDBSaveFiles();
            string filePath;
            if (existingFiles.Count < numOfBackups)
                filePath = Path.Combine(DBDir, string.Format(fileNameFormat, existingFiles.Count + 1));
            else
                filePath = existingFiles[0].FullName;

            using (TextWriter writer = new StreamWriter(filePath))
                SerializeToStream(writer);

            string dailyBackupFilePath = Path.Combine(DBDir, "Daily Backup.xml");
            if (!File.Exists(dailyBackupFilePath) || (DateTime.Now - new FileInfo(dailyBackupFilePath).LastWriteTime).TotalDays > 1.0)
            {
                using (TextWriter writer = new StreamWriter(dailyBackupFilePath))
                    SerializeToStream(writer);
            }
        }

        public override void DeserializeFromString(string str)
        {
            using (StringReader reader = new StringReader(str))
                DeserializeFromStream(reader);
        }

        private void DeserializeFromStream(TextReader reader)
        {
            XmlSerializer xmlSerial = new XmlSerializer(typeof(SneezeDatabase));
            SneezeDatabase db = (SneezeDatabase)xmlSerial.Deserialize(reader);
            Sneezes = db.Sneezes;
            IdToUser = db.IdToUser;
            CountdownStart = db.CountdownStart;
        }

        private IReadOnlyList<FileInfo> GetDBSaveFiles()
        {
            List<FileInfo> existingFiles = new List<FileInfo>(numOfBackups);
            for (int fileNum = 1; fileNum <= numOfBackups; fileNum++)
            {
                string filePath = Path.Combine(DBDir, string.Format(fileNameFormat, fileNum));
                if (File.Exists(filePath))
                {
                    try
                    {
                        existingFiles.Add(new FileInfo(filePath));
                    }
                    catch (IOException) { }
                }
            }

            existingFiles.Sort((i1, i2) => i1.LastWriteTime.CompareTo(i2.LastWriteTime));

            return existingFiles;
        }
    }
}
