using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatClient.Net.IO
{
    class PacketReader : BinaryReader
    {
        private NetworkStream _ns;

        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }

        public string ReadMessage()
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = new byte[length];
            _ns.Read(msgBuffer, 0, length);

            var msg = Encoding.ASCII.GetString(msgBuffer);
            return msg;
        }

        public byte[] ReadFile()
        {
            // Read the length of the file data as a 4-byte integer
            int length = ReadInt32();

            // Read the file data itself
            byte[] fileData = new byte[length];
            Read(fileData, 0, length);

            return fileData;
        }
    }
}
