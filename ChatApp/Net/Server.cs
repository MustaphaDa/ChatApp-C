using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using ChatClient.Net.IO; // Import the PacketBuilder Class 

namespace ChatClient.Net
{
    class Server
    {
        TcpClient _client;
        public PacketReader PacketReader;

        public event Action connectedEvent;
        public event Action msgReceivedEvent;
        public event Action privateMsgReceivedEvent;
        public event Action userDisconnectEvent;
        public event Action fileReceivedEvent;
        public event Action privateFileReceivedEvent;

        public Server()
        {
            _client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 7891);
                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;

                        case 5:
                            msgReceivedEvent?.Invoke();
                            break;

                        case 6:
                            privateMsgReceivedEvent?.Invoke();
                            break;


                        case 8:
                            fileReceivedEvent?.Invoke();
                            break;

                        case 10:
                            userDisconnectEvent?.Invoke();
                            break;

                        default:
                            Console.WriteLine("Unknown opcode: " + opcode);
                            break;
                    }
                }
            });
        }

        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }

        public void SendFileToServer(string filePath)
        {
            try
            {
                // Send only the file path
                var filePacket = new PacketBuilder();
                filePacket.WriteOpCode(8);
                filePacket.WriteMessage(filePath);
                _client.Client.Send(filePacket.GetPacketBytes());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending file: " + ex.Message);
            }
        }

         public void SendPrivateMessageToUser(string recipientUID, string message)
        {
            var privateMessagePacket = new PacketBuilder();
            privateMessagePacket.WriteOpCode(6);
            privateMessagePacket.WriteMessage(recipientUID);
            privateMessagePacket.WriteMessage(message);
            _client.Client.Send(privateMessagePacket.GetPacketBytes());
        }

      
    }
}
