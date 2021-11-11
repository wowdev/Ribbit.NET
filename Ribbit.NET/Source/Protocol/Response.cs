using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

using MimeKit;
using MimeKit.Cryptography;
using System.Security.Cryptography;

namespace Ribbit.Protocol
{
    public class Response
    {
        private static string SignatureChunkName;

        public struct Chunk
        {
            public Dictionary<string, string> headers { get; private set; }
            public Stream contents { get; private set; }
            public string name { get; private set; }

            public Chunk(MimePart entity)
            {
                this.headers = entity.Headers.ToDictionary(header => header.Id.ToHeaderName(), header => header.Value);
                this.contents = entity.Content.Stream;
                this.name = this.headers["Content-Disposition"];
            }
        }

        public MimeMessage message = null;

        public byte[] byteContent { get; private set; }

        public Dictionary<string, Chunk> chunks { get; private set; }
        public Dictionary<string, string> headers { get; private set; }
        public string Name { get; private set; }

        public Stream data => this.chunks.First().Value.contents;
        public Stream signature => this[SignatureChunkName].contents;

        public Response(string raw): this(new MemoryStream(Encoding.ASCII.GetBytes(raw)))
        {
        }

        public Response(Stream stream)
        {
            this.byteContent = new byte[stream.Length];
            stream.Read(this.byteContent, 0, (int)stream.Length);
            stream.Position = 0;
            this.message = MimeMessage.Load(stream);
            this.Name = this.message.Subject;

            if (this.message.Body is Multipart multipart)
            {
                this.chunks = multipart.Select(part => new Chunk(part as MimePart)).ToDictionary(chunk => chunk.name, chunk => chunk);
                this.headers = this.message.Headers.ToDictionary(header => header.Id.ToHeaderName(), header => header.Value);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool IsValid()
        {
            if(this.message.Body is Multipart multipart)
            {
                var epilogue = multipart.Epilogue;
                if (string.IsNullOrEmpty(epilogue))
                    return false;

                var splitEpilogue = epilogue.Split(':');
                if (splitEpilogue.Length < 2)
                    return false;

                var checksum = splitEpilogue[1].Trim();
                if (checksum.Length != 64)
                    return false;

                string computedChecksum;

                using (SHA256 sha256Hash = SHA256.Create())
                {
                    var messageContentsWithoutEpilogue = this.byteContent.Take(this.byteContent.Length - 76).ToArray();
                    byte[] bytes = sha256Hash.ComputeHash(messageContentsWithoutEpilogue);
                    computedChecksum = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                }

                if (computedChecksum.Equals(checksum))
                    return true;
            }
            else
            {
                return false;
            }

            return false;
        }

        public Chunk this[string key]
        {
            get
            {
                return this.chunks[key];
            }
        }

        public override string ToString() {
            var memoryStream = new MemoryStream();
            this.data.CopyTo(memoryStream);

            var bytes = memoryStream.GetBuffer();
            var content = Encoding.ASCII.GetString(bytes); 

            return content;
        }
    }
}
