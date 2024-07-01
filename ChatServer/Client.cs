using ChatClient.Net.IO;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatServer
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packetReader;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());

            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username: {Username}");

            Task.Run(() => Process());
        }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            // Regular message handling
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Message received! {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: {msg}");
                            break;

                        case 6:
                            // Handle private message
                            var recipientUID = _packetReader.ReadMessage();
                            var privateMsg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Private message received from {Username} to {recipientUID}: {privateMsg}");
                            Program.SendPrivateMessage(recipientUID, $"[{DateTime.Now}]: (Private) [{Username}]: {privateMsg}");
                            break;

                        case 8:
                            // Handle file path transfer to the server
                            var filePath = _packetReader.ReadMessage();
                            string filecontent = File.ReadAllText(filePath);
                            Console.WriteLine($"[{DateTime.Now}]: File path received from {Username} of the content\n: {filecontent}");
                            Program.SendFileToAll(filePath); // Just send the file path to the server
                            break;

                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"[{Username.ToString()}]: Disconnected!");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
