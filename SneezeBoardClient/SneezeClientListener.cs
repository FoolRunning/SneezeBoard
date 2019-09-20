using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using NetworkCommsDotNet.Connections.UDP;
using SneezeBoardCommon;

namespace SneezeBoardClient
{
    public static class SneezeClientListener
    {
        public static SneezeDatabase Database;

        public static event Action FailedToConnect;
        public static event Action ConnectionClosed;
        public static event Action ConnectionOpened;
        public static event Action GotDatabase;
        public static event Action DatabaseUpdated;
        public static event Action<string> PersonSneezed;
        public static event Action UserUpdated;

        public static string Ip;

        public static void StartListener(string ip)
        {
            Ip = ip;
            Thread t = new Thread(GetDatabase);
            t.IsBackground = true;
            t.Name = "Listener Thread";
            t.Start();
            
        }

        private static void GetDatabase()
        {
	        NetworkComms.AppendGlobalIncomingPacketHandler<string>(Messages.PersonSneezed, HandlePersonSneezed);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(Messages.UserUpdated, HandleUserUpdated);
            NetworkComms.AppendGlobalConnectionCloseHandler(HandleConnectionClosed);

			Database = GetFromServer<SneezeDatabase>(Messages.DatabaseRequested, Messages.DatabaseObject);

            if(Database != null)
                GotDatabase?.Invoke();
            //We have used comms so we make sure to call shutdown
            //NetworkComms.Shutdown();
            ConnectionOpened?.Invoke();
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

        private static T1 GetFromServer<T1>(string sendMessage, string returnMessage, string serializedObject = "") where T1 : ServerObject, new()
        {
            ConnectionInfo serverConnectionInfo;
            if (Ip == null)
                return default(T1);
            try
            {
                serverConnectionInfo = new ConnectionInfo(Ip, CommonInfo.ServerPort);
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
                
                string serializedObj = String.Empty;
                if (serializedObject == "")
	                serializedObj = serverConnection.SendReceiveObject<string>(sendMessage, returnMessage,  10000);
                else
	                serializedObj = serverConnection.SendReceiveObject<string, string>(sendMessage, returnMessage, 10000, serializedObject);

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
            if (Ip == null)
                return;
            try
            {
                serverConnectionInfo = new ConnectionInfo(Ip, CommonInfo.ServerPort);
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

        private static void HandlePersonSneezed(PacketHeader header, Connection connection, string serializedSneeze)
        {
	        SneezeRecord sneeze = new SneezeRecord();
	        sneeze.DeserializeFromString(serializedSneeze);
			Database?.Sneezes.Add(sneeze);
            PersonSneezed?.Invoke(Database?.IdToUser[sneeze.UserId].Name);
        }

        private static void HandleUserUpdated(PacketHeader header, Connection connection, string serializedUser)
        {
	        UserInfo user = new UserInfo();
	        user.DeserializeFromString(serializedUser);
	        if (Database != null)
	        {
		        UserInfo dbUser = Database.Users.FirstOrDefault(x => x.UserGuid == user.UserGuid);
		        if (dbUser == null)
		        {
			        UserInfo[] dbUsers = Database.Users;
			        Array.Resize(ref dbUsers, dbUsers.Length + 1);
			        dbUsers[dbUsers.Length - 1] = user;
                }
		        else
		        {
			        dbUser.Color = user.Color;
			        dbUser.Name = user.Name;
		        }
	        }
        }

        private static void HandleConnectionClosed(Connection connection)
        {
            Database = null;
            ConnectionClosed?.Invoke();
        }
    }
}
