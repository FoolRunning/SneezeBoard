using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SneezeBoardCommon
{
    public class SneezeDatabase : ServerObject
    {
       public const int currentVersionNumber = 1;

        private const string fileNameFormat = "database{0}.xml";
        private const int numOfBackups = 4;
        private static string DBDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SneezeBoard");

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

        public Dictionary<Guid, UserStats> FindUserStats()
        {
            Dictionary<Guid, UserStats> userSneezeCountMap = new Dictionary<Guid, UserStats>(IdToUser.Count);
            foreach (SneezeRecord sneeze in Sneezes)
            {
                UserStats stats;
                if (!userSneezeCountMap.TryGetValue(sneeze.UserId, out stats))
                    userSneezeCountMap[sneeze.UserId] = stats = new UserStats();

                stats.TotalSneezes++;
                if (stats.FirstSneezeDate == DateTime.MinValue)
                    stats.FirstSneezeDate = sneeze.Date;
                stats.LastSneezeDate = sneeze.Date;
            }

            return userSneezeCountMap;
        }

        public Dictionary<Guid, int> FindLongestStreaks()
        {
            Dictionary<Guid, int> streakMap = new Dictionary<Guid, int>(IdToUser.Count);

            Guid currentUserId = Sneezes[0].UserId; // Start with the first person
            int currentStreak = 0; // Will automatically be incremented to 1 with the first user's first sneeze
            int maxStreak;
            foreach (SneezeRecord sneeze in Sneezes)
            {
                if (currentUserId == sneeze.UserId && sneeze.UserId != CommonInfo.UnknownUserId)
                {
                    currentStreak++;
                }
                else
                {
                    streakMap.TryGetValue(currentUserId, out maxStreak);
                    if (currentStreak > maxStreak)
                        streakMap[currentUserId] = currentStreak;

                    currentStreak = 1;
                    currentUserId = sneeze.UserId;
                }
            }

            streakMap.TryGetValue(currentUserId, out maxStreak);
            if (currentStreak > maxStreak)
                streakMap[currentUserId] = currentStreak;

            return streakMap;
        }

        public Dictionary<int, List<Guid>> FindAllStreaks()
        {
            Dictionary<int, List<Guid>> streakMap = new Dictionary<int, List<Guid>>();

            Guid currentUserId = Sneezes[0].UserId; // Start with the first person
            int currentStreak = 0; // Will automatically be incremented to 1 with the first user's first sneeze
            List<Guid> streakUsers;
            foreach (SneezeRecord sneeze in Sneezes)
            {
                if (currentUserId == sneeze.UserId && sneeze.UserId != CommonInfo.UnknownUserId)
                {
                    currentStreak++;
                }
                else
                {
                    if (!streakMap.TryGetValue(currentStreak, out streakUsers))
                        streakMap[currentStreak] = streakUsers = new List<Guid>();
                    streakUsers.Add(currentUserId);

                    currentStreak = 1;
                    currentUserId = sneeze.UserId;
                }
            }

            if (!streakMap.TryGetValue(currentStreak, out streakUsers))
                streakMap[currentStreak] = streakUsers = new List<Guid>();
            streakUsers.Add(currentUserId);

            return streakMap;
        }

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
            
            // If database needs to go through some kind of upgrade process, do that here.

            Sneezes = db.Sneezes;
            IdToUser = db.IdToUser;
            CountdownStart = db.CountdownStart;
            Version = currentVersionNumber;
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

    public class UserStats
    {
        public int TotalSneezes;
        public DateTime FirstSneezeDate = DateTime.MinValue;
        public DateTime LastSneezeDate = DateTime.MinValue;
    }
}
