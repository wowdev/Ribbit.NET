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

        private TcpClient socket;
        private NetworkStream stream;

        public Client(Region region) : this(region.GetHostname(), 1119) 
        { 

        }

        public Client(string host, short port)
        {
            this.host = host;
            this.port = port;

            this.socket = new TcpClient();
        }

        public void Connect()
        {
            this.socket.Connect(this.host, this.port);

            this.stream = this.socket.GetStream();
        }

        public string Request(string endpoint)
        {
            var command = Encoding.ASCII.GetBytes(endpoint + Environment.NewLine);
            this.stream.Write(command);

            var writer = new MemoryStream();
            byte[] buffer = new byte[this.socket.ReceiveBufferSize];

            do
            {
                int readBytes = this.stream.Read(buffer, 0, buffer.Length);

                if (readBytes <= 0)
                {
                    break;
                }

                writer.Write(buffer, 0, readBytes);
            } while (this.stream.DataAvailable);

            return Encoding.ASCII.GetString(writer.GetBuffer());
        }
    }
}
