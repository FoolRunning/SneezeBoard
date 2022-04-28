using System;
using System.Drawing;
using System.Threading;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using SneezeBoardCommon;

namespace SneezeBoardServer
{
    class Server
    {
        private static SneezeDatabase database = new SneezeDatabase();
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            if (!database.Load())
                database.IdToUser.Add(CommonInfo.UnknownUserId, new UserInfo("Nemo", CommonInfo.UnknownUserId, Color.Sienna));
            //DO NOT DELETE! May need this later.
            //else
            //{
            //    for (int i = 0; i < database.Sneezes.Count; i++)
            //    {
            //        int sneezeNum = database.CountdownStart - i;
            //        database.Sneezes[i].Date = CommonInfo.GetDateOfSneeze(sneezeNum);
            //    }
            //}

            //Trigger the method HandleSneeze when a packet of type 'Message' is received
            //We expect the incoming object to be a string which we state explicitly by using <string>
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(Messages.Sneeze, HandleSneeze);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(Messages.AddUser, HandleAddUser);
            NetworkComms.AppendGlobalIncomingPacketHandler<int>(Messages.DatabaseRequested, HandleDatabaseRequest);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(Messages.UpdateUser, HandleUpdateUser);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(Messages.UpdateSneeze, HandleUpdateSneeze);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(Messages.RemoveSneeze, HandleRemoveSneeze);
            //Start listening for incoming connections
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, CommonInfo.ServerPort));

            //Print out the IPs and ports we are now listening on
            Console.WriteLine("Server listening for TCP connection on:");
            foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
                Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);

            
            //Let the user close the server
            Console.WriteLine("\nPress Ctrl+C to close server.");

            _quitEvent.WaitOne();
            
            Console.WriteLine("Shutting Down");
            //We have used NetworkComms so we should ensure that we correctly call shutdown
            NetworkComms.Shutdown();
        }

        /// <summary>
        /// Writes the provided sneeze to the console window
        /// </summary>
        private static void HandleSneeze(PacketHeader header, Connection connection, string serializedSneeze)
        {
            SneezeRecord sneeze = new SneezeRecord();
            sneeze.DeserializeFromString(serializedSneeze);
            database.Sneezes.Add(sneeze);
            database.Save();

            foreach (ConnectionInfo info in NetworkComms.AllConnectionInfo())
            {
                TCPConnection.GetConnection(info).SendObject(Messages.PersonSneezed, serializedSneeze);
            }
        }

        private static void HandleAddUser(PacketHeader header, Connection connection, string serializedUser)
        {
            UserInfo user = new UserInfo();
            user.DeserializeFromString(serializedUser);
            database.IdToUser.Add(user.UserGuid, user);
            database.Save();

            TellClientToUpdateUsers(user);
        }

        private static void HandleUpdateSneeze(PacketHeader header, Connection connection, string serializedSneeze)
        {
            SneezeRecord sneeze = new SneezeRecord();
            sneeze.DeserializeFromString(serializedSneeze);
            int sneezeIndex = database.Sneezes.FindIndex(s => s.Date == sneeze.Date);
            if (sneezeIndex == -1)
                return; // This should never happen, but just in case...

            database.Sneezes[sneezeIndex] = sneeze;
            database.Save();

            TellClientToUpdateSneeze(sneeze);
        }

        private static void HandleUpdateUser(PacketHeader header, Connection connection, string serializedUser)
        {
            UserInfo user = new UserInfo();
            user.DeserializeFromString(serializedUser);
            database.IdToUser[user.UserGuid].Color = user.Color;
            database.Save();

            TellClientToUpdateUsers(user);
        }

        private static void HandleRemoveSneeze(PacketHeader header, Connection connection, string serializedSneeze)
        {
            SneezeRecord sneeze = new SneezeRecord();
            sneeze.DeserializeFromString(serializedSneeze);
            var sneezeRecord = database.Sneezes.Find(s => s.Date == sneeze.Date);
            if (sneezeRecord != null)
            {
                database.Sneezes.Remove(sneezeRecord);
                database.Save();

                //We could send the entire database, but let's try to avoid that if possible.
                TellClientToRemoveSneeze(sneeze);
            }
        }

        private static void HandleDatabaseRequest(PacketHeader header, Connection connection, int message)
        {
            connection.SendObject(Messages.DatabaseObject, database.SerializeToString());
        }

        private static void TellClientToUpdateSneeze(SneezeRecord sneeze)
        {
            string serializedSneeze = sneeze.SerializeToString();
	        foreach (ConnectionInfo info in NetworkComms.AllConnectionInfo())
	        {
		        TCPConnection.GetConnection(info).SendObject(Messages.SneezeUpdated, serializedSneeze);
	        }
        }

        private static void TellClientToRemoveSneeze(SneezeRecord sneeze)
        {
            string serializedSneeze = sneeze.SerializeToString();
            foreach (ConnectionInfo info in NetworkComms.AllConnectionInfo())
            {
                TCPConnection.GetConnection(info).SendObject(Messages.SneezeRemoved, serializedSneeze);
            }
        }

        private static void TellClientToUpdateUsers(UserInfo userInfo)
        {
            string serializedUser = userInfo.SerializeToString();
            foreach (ConnectionInfo info in NetworkComms.AllConnectionInfo())
            {
                TCPConnection.GetConnection(info).SendObject(Messages.UserUpdated, serializedUser);
            }
        }
    }
}
