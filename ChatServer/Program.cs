using ChatClient.Net.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    class Program
    {
        static List<Client> _users;
        static TcpListener _listener;

        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();

            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);

                /* Broadcast the connection to everyone on the server */
                BroadcastConnection();
            }
        }

        static void BroadcastConnection()
        {
            foreach (var user in _users)
            {
                foreach (var usr in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastMessage(string message)
        {
            foreach (var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = _users.FirstOrDefault(x => x.UID.ToString() == uid);
            if (disconnectedUser != null)
            {
                _users.Remove(disconnectedUser);

                foreach (var user in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(10);
                    broadcastPacket.WriteMessage(uid);
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }

                BroadcastMessage($"[{disconnectedUser.Username}] Disconnected!");
            }
        }

        // Method to send private message to a specific user
        public static void SendPrivateMessage(string recipientUID, string message)
        {
            var recipient = _users.FirstOrDefault(x => x.UID.ToString() == recipientUID);
            if (recipient != null)
            {
                var privateMessagePacket = new PacketBuilder();
                privateMessagePacket.WriteOpCode(6); // Opcode for private messages
                privateMessagePacket.WriteMessage(message);
                recipient.ClientSocket.Client.Send(privateMessagePacket.GetPacketBytes());
            }
        }

        // Method to handle file transfer
      

        public static void SendFileToAll(string filePath)
        {
            var fileContent = "";
            try
            {
                fileContent = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                fileContent = $"Error reading file: {ex.Message}";
            }

            foreach (var user in _users)
            {
                var filePacket = new PacketBuilder();
                filePacket.WriteOpCode(8); // Opcode for file transfer to all
                filePacket.WriteMessage(filePath);
                filePacket.WriteMessage(fileContent);
                user.ClientSocket.Client.Send(filePacket.GetPacketBytes());
            }
        }
    }
}
