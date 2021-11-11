using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

using Ribbit.Constants;

namespace Ribbit.Protocol
{
    public class Client
    {
        private string host;
        private short port;


        public Client(Region region) : this(region.GetHostname(), 1119) 
        { 

        }

        public Client(string host, short port)
        {
            this.host = host;
            this.port = port;
        }

        public Response Request(string endpoint)
        {

            var socket = new TcpClient(this.host, this.port);
            var stream = socket.GetStream();

            var command = Encoding.ASCII.GetBytes(endpoint + Environment.NewLine);
            stream.Write(command, 0, command.Length);

            var responseBuffer = new MemoryStream();
            do
            {
                byte[] chunkBuffer = new byte[socket.ReceiveBufferSize];
                int readBytes = stream.Read(chunkBuffer, 0, chunkBuffer.Length);

                if (readBytes <= 0)
                {
                    break;
                }

                responseBuffer.Write(chunkBuffer, 0, readBytes);
            } while (true);

            var dataStream = (Stream)responseBuffer;
            dataStream.Position = 0;

            socket.Close();

            return new Response(dataStream);
        }
    }
}
