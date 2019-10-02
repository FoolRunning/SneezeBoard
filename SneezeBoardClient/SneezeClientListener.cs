using System;
using System.Threading;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using SneezeBoardCommon;

namespace SneezeBoardClient
{
    public enum DatabaseErrorType
    {
        None,
        VersionNumberConflict,
    }

    public static class SneezeClientListener
    {
        public static event Action FailedToConnect;
        public static event Action ConnectionClosed;
        public static event Action ConnectionOpened;
        public static event Action GotDatabase;
        public static event Action<DatabaseErrorType> DatabaseError;
        public static event Action DatabaseUpdated;
        public static event Action<SneezeRecord> PersonSneezed;
        public static event Action UserUpdated;
        public static event Action SneezeUpdated;

        private static readonly object dbSync = new object();
        private static SneezeDatabase database;
        private static string ip;

        static SneezeClientListener()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(Messages.PersonSneezed, HandlePersonSneezed);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(Messages.UserUpdated, HandleUserUpdated);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(Messages.SneezeUpdated, HandleSneezeUpdated);
            NetworkComms.AppendGlobalConnectionCloseHandler(HandleConnectionClosed);
        }

        public static SneezeDatabase Database
        {
            get
            {
                lock (dbSync)
                    return database;
            }
        }

        public static void StartListener(string ip)
        {
            NetworkComms.CloseAllConnections();

            SneezeClientListener.ip = ip;
            Thread t = new Thread(GetDatabase);
            t.IsBackground = true;
            t.Name = "Listener Thread";
            t.Start();
        }

        public static void ShutDown()
        {
            NetworkComms.Shutdown();
        }

        private static void GetDatabase()
        {
            lock (dbSync)
            {
                database = GetFromServer<SneezeDatabase>(Messages.DatabaseRequested, Messages.DatabaseObject);
                ConnectionOpened?.Invoke();
                if (!VerifyDatabase())
                    return;
            }

            GotDatabase?.Invoke();
        }

        public static void UpdateUser(UserInfo user)
        {
            SendToServer(Messages.UpdateUser, user);
        }

        public static void AddUser(UserInfo user)
        {
            SendToServer(Messages.AddUser, user);
        }

        public static void Sneeze(SneezeRecord sneeze)
        {
            SendToServer(Messages.Sneeze, sneeze);
        }

        public static void UpdateSneeze(SneezeRecord sneeze)
        {
            SendToServer(Messages.UpdateSneeze, sneeze);
        }

        private static bool VerifyDatabase()
        {
            if (database == null)
                return false;

            if (database.Version != SneezeDatabase.currentVersionNumber)
            {
                NetworkComms.CloseAllConnections();
                database = null;
                DatabaseError?.Invoke(DatabaseErrorType.VersionNumberConflict);
                return false;
            }

            return true;
        }

        private static T1 GetFromServer<T1>(string sendMessage, string returnMessage, string serializedObject = "") where T1 : ServerObject, new()
        {
            ConnectionInfo serverConnectionInfo;
            if (ip == null)
                return default(T1);
            try
            {
                serverConnectionInfo = new ConnectionInfo(ip, CommonInfo.ServerPort);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //ShowMessage("Failed to parse the server IP and port. Please ensure it is correct and try again");
                return default(T1);
            }

            try
            {
                TCPConnection serverConnection = TCPConnection.GetConnection(serverConnectionInfo);
                
                string serializedObj;
                if (serializedObject == "")
	                serializedObj = serverConnection.SendReceiveObject<string>(sendMessage, returnMessage,  30000);
                else
	                serializedObj = serverConnection.SendReceiveObject<string, string>(sendMessage, returnMessage, 30000, serializedObject);

                T1 obj = new T1();
                obj.DeserializeFromString(serializedObj);
                return obj;
            }
            catch (CommsException e)
            {
                Console.WriteLine(e.ToString());
                /*AppendLineToChatHistory("Error: A communication error occurred while trying to send message to " + serverConnectionInfo + ". Please check settings and try again.");*/

                FailedToConnect?.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                /*AppendLineToChatHistory("Error: A general error occurred while trying to send message to " + serverConnectionInfo + ". Please check settings and try again.");*/
            }

            return default(T1);
        }

        private static void SendToServer<T>(string sendMessage, T objectToSend) where T : ServerObject
        {
            ConnectionInfo serverConnectionInfo;
            if (ip == null)
                return;
            try
            {
                serverConnectionInfo = new ConnectionInfo(ip, CommonInfo.ServerPort);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //ShowMessage("Failed to parse the server IP and port. Please ensure it is correct and try again");
                return;
            }

            try
            {
                TCPConnection serverConnection = TCPConnection.GetConnection(serverConnectionInfo);
                serverConnection.SendObject(sendMessage, objectToSend.SerializeToString());
            }
            catch (CommsException e)
            {
                Console.WriteLine(e.ToString());
                /*AppendLineToChatHistory("Error: A communication error occurred while trying to send message to " + serverConnectionInfo + ". Please check settings and try again.");*/
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                /*AppendLineToChatHistory("Error: A general error occurred while trying to send message to " + serverConnectionInfo + ". Please check settings and try again.");*/
            }
        }

        private static void HandleUserUpdated(PacketHeader header, Connection connection, string serializedUser)
        {
            lock (dbSync)
            {
                UserInfo user = new UserInfo();
                user.DeserializeFromString(serializedUser);
                if (database != null)
                    database.IdToUser[user.UserGuid] = user;
            }

            UserUpdated?.Invoke();
        }

        private static void HandleSneezeUpdated(PacketHeader header, Connection connection, string serializedSneeze)
        {
            lock (dbSync)
            {
                if (database == null)
                    return;

                SneezeRecord sneeze = new SneezeRecord();
                sneeze.DeserializeFromString(serializedSneeze);
                int sneezeIndex = database.Sneezes.FindIndex(s => s.Date == sneeze.Date);
                if (sneezeIndex == -1)
                    return; // This should never happen, but just in case...

                database.Sneezes[sneezeIndex] = sneeze;
            }

            SneezeUpdated?.Invoke();
        }

        private static void HandlePersonSneezed(PacketHeader header, Connection connection, string serializedSneeze)
        {
            SneezeRecord sneeze;
            lock (dbSync)
            {
                sneeze = new SneezeRecord();
                sneeze.DeserializeFromString(serializedSneeze);
                Database?.Sneezes.Add(sneeze);
            }

            PersonSneezed?.Invoke(sneeze);
        }

        private static void HandleConnectionClosed(Connection connection)
        {
            lock (dbSync)
                database = null;
            ConnectionClosed?.Invoke();
        }
    }
}
