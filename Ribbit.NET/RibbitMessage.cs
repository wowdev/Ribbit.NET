using MimeKit;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Ribbit.NET
{
    public class RibbitMessage
    {
        public MimeMessage parsed = new MimeMessage();

        public RibbitMessage(string message)
        {
            Message(message);
        }

        public string Message(string message)
        {
            TcpClient client = new TcpClient("us.version.battle.net", 1119);

            var data = Encoding.ASCII.GetBytes(message + "\r\n");

            var stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            stream.Flush();

            if (stream.CanRead)
            {
                byte[] readBuffer = new byte[client.ReceiveBufferSize];
                using (var writer = new MemoryStream())
                {
                    do
                    {
                        int numberOfBytesRead = stream.Read(readBuffer, 0, readBuffer.Length);
                        if (numberOfBytesRead <= 0)
                        {
                            break;
                        }
                        writer.Write(readBuffer, 0, numberOfBytesRead);
                    } while (stream.DataAvailable);
                    writer.Position = 0;
                    parsed = MimeMessage.Load(writer);
                }
            }

            stream.Close();

            return parsed.TextBody;
        }
    }
}
