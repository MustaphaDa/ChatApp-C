using System;
using System.IO;
using System.Text;

namespace ChatClient.Net.IO
{
    class PacketBuilder
    {
        MemoryStream _ms;
        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WriteOpCode(byte opcode)
        {
            _ms.WriteByte(opcode);
        }

        public void WriteMessage(string msg)
        {
            var msgLength = msg.Length;
            _ms.Write(BitConverter.GetBytes(msgLength));
            _ms.Write(Encoding.ASCII.GetBytes(msg));
        }

        

        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }
    }
} // so the PacketBuilder convert the message and the message lenght and the opcode to the byte using SCII encoded bytes and store this bytes into array 
