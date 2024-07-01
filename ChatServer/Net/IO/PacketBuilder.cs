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

        public void WriteFile(byte[] fileData)
        {
            // Write the length of the file data as a 4-byte integer
            _ms.Write(BitConverter.GetBytes(fileData.Length), 0, 4);

            // Write the file data itself
            _ms.Write(fileData, 0, fileData.Length);
        }

        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }
    }
} // so the PacketBuilder convert the message and the message lenght and the opcode to the byte using SCII encoded bytes and store this bytes into array 
