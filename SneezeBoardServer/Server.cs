using System;
using System.Drawing;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using SneezeBoardCommon;

namespace SneezeBoardServer
{
    class Server
    {
        private static SneezeDatabase database = new SneezeDatabase();

        static void Main(string[] args)
        {
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
            //Start listening for incoming connections
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, CommonInfo.ServerPort));

            //Print out the IPs and ports we are now listening on
            Console.WriteLine("Server listening for TCP connection on:");
            foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
                Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);

            //Let the user close the server
            Console.WriteLine("\nPress any key to close server.");
            Console.ReadKey(true);

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

            TellClientsToUpdate();
            foreach (ConnectionInfo info in NetworkComms.AllConnectionInfo())
            {
                TCPConnection.GetConnection(info).SendObject(Messages.PersonSneezed, database.IdToUser[sneeze.UserId].Name);
            }
        }

        private static void HandleAddUser(PacketHeader header, Connection connection, string serializedUser)
        {
            UserInfo user = new UserInfo();
            user.DeserializeFromString(serializedUser);
            database.IdToUser.Add(user.UserGuid, user);
            database.Save();

            TellClientsToUpdate();
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

            TellClientsToUpdate();
        }

        private static void HandleUpdateUser(PacketHeader header, Connection connection, string serializedUser)
        {
            UserInfo user = new UserInfo();
            user.DeserializeFromString(serializedUser);
            database.IdToUser[user.UserGuid].Color = user.Color;
            database.Save();

            TellClientsToUpdate();
        }

        private static void HandleDatabaseRequest(PacketHeader header, Connection connection, int message)
        {
            connection.SendObject(Messages.DatabaseObject, database.SerializeToString());
        }

        private static void TellClientsToUpdate()
        {
            string dbSerialized = database.SerializeToString();
            foreach (ConnectionInfo info in NetworkComms.AllConnectionInfo())
            {
                TCPConnection.GetConnection(info).SendObject(Messages.DatabaseObject, dbSerialized);
            }
        }
    }
}
