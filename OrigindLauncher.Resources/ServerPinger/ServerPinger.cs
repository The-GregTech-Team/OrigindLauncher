// Code from GoodTimeStudio/Minecraft-Server-Status-Checker/ServerPinger/
// https://github.com/GoodTimeStudio/Minecraft-Server-Status-Checker/tree/master/ServerPinger
// 恕我直言 这代码太烂了(

// The MIT License (MIT)

// Copyright(c) 2015 GoodTime Studio

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace GoodTimeStudio.ServerPinger
{

    public class ServerPinger
    {
        public int timeout = 8000;

        public string ServerName;
        public string ServerAddress;
        public int ServerPort;
        public PingVersion ServerVersion;

        public ServerPinger(string ServerName, string ServerAddress,
            int ServerPort, PingVersion ServerVersion)
        {
            this.ServerName = ServerName;
            this.ServerAddress = ServerAddress;
            this.ServerPort = ServerPort;
            this.ServerVersion = ServerVersion;
        }

        public async Task<ServerStatus> GetStatus()
        {
            try
            {
                if (ServerVersion == PingVersion.MC_Current)
                {
                    return await GetStatusCurrent();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private async Task<ServerStatus> GetStatusCurrent()
        {
            try
            {
                using (var socket = new System.Net.Sockets.TcpClient())
                {
                    await socket.ConnectAsync(ServerAddress, ServerPort);
                    BinaryWriter writer;

                    #region handshake
                    MemoryStream handshakeStream = new MemoryStream();
                    BinaryWriter handshakewriter = new BinaryWriter(handshakeStream);

                    handshakewriter.Write((byte)0x00);  // Packet ID
                    // Protocol version, http://wiki.vg/Protocol_version_numbers
                    handshakewriter.Write(VarintHelper.IntToVarint(210));
                    handshakewriter.Write(GetByteFromString(ServerAddress)); // hostname or IP
                    handshakewriter.Write((short)ServerPort); // Port
                    handshakewriter.Write(VarintHelper.IntToVarint(0x01)); // Next state, 1 for `status'
                    handshakewriter.Flush();

                    writer = new BinaryWriter(socket.GetStream());
                    writer.Write(VarintHelper.IntToVarint((int)handshakeStream.Length));
                    writer.Write(handshakeStream.ToArray());
                    writer.Flush();
                    #endregion

                    writer = new BinaryWriter(socket.GetStream());
                    /* BE: 0x0100, Length and writer.Write((byte)0x00);
                     * ID for `Request'
                     */
                    writer.Write((short)0x0001);
                    writer.Flush();
                    var streamIn = socket.GetStream();
                    BinaryReader reader = new BinaryReader(streamIn);
                    var packetLen = VarintHelper.ReadVarInt(reader);
                    var packetId = VarintHelper.ReadVarInt(reader);
                    var packetJsonLen = VarintHelper.ReadVarInt(reader);
                    var response = reader.ReadBytes(packetJsonLen);
                    string json = Encoding.UTF8.GetString(response);
                    Debug.WriteLine(json);
                    return JsonConvert.DeserializeObject<ServerStatus>(json);
                }
            }
            catch
            {
                return null;
            }

        }

        private byte[] GetByteFromString(string content)
        {
            List<byte> output = new List<byte>();

            output.AddRange(VarintHelper.IntToVarint(content.Length));
            output.AddRange(Encoding.UTF8.GetBytes(content));

            return output.ToArray();
        }
    }
}
