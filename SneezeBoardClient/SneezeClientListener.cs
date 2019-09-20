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
        private const int currentVersionNumber = 0;

        public static event Action FailedToConnect;
        public static event Action ConnectionClosed;
        public static event Action GotDatabase;
        public static event Action<DatabaseErrorType> DatabaseError;
        public static event Action DatabaseUpdated;
        public static event Action<string> PersonSneezed;

        private static readonly object dbSync = new object();
        private static SneezeDatabase database;
        private static string ip;

        static SneezeClientListener()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(Messages.DatabaseObject, HandleSneezeDatabase);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(Messages.PersonSneezed, HandlePersonSneezed);
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

            if (database.Version != currentVersionNumber)
            {
                NetworkComms.CloseAllConnections();
                database = null;
                DatabaseError?.Invoke(DatabaseErrorType.VersionNumberConflict);
                return false;
            }

            return true;
        }

        private static T GetFromServer<T>(string sendMessage, string returnMessage) where T : ServerObject, new()
        {
            ConnectionInfo serverConnectionInfo;
            if (ip == null)
                return default(T);
            try
            {
                serverConnectionInfo = new ConnectionInfo(ip, CommonInfo.ServerPort);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //ShowMessage("Failed to parse the server IP and port. Please ensure it is correct and try again");
                return default(T);
            }

            try
            {
                TCPConnection serverConnection = TCPConnection.GetConnection(serverConnectionInfo);
                
                string serializedObj = serverConnection.SendReceiveObject<string>(
                    sendMessage, returnMessage, 10000);

                T obj = new T();
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

            return default(T);
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

        private static void HandleSneezeDatabase(PacketHeader header, Connection connection, string serializedDatabase)
        {
            lock (dbSync)
            {
                database = new SneezeDatabase();
                database.DeserializeFromString(serializedDatabase);
                if (!VerifyDatabase())
                    return;
            }

            DatabaseUpdated?.Invoke();
        }

        private static void HandlePersonSneezed(PacketHeader header, Connection connection, string name)
        {
            PersonSneezed?.Invoke(name);
        }

        private static void HandleConnectionClosed(Connection connection)
        {
            lock (dbSync)
                database = null;
            ConnectionClosed?.Invoke();
        }
    }
}
