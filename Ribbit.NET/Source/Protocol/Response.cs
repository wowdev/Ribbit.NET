using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

using MimeKit;
using MimeKit.Cryptography;

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
            this.message = MimeMessage.Load(stream);
            this.Name = this.message.Subject;

            if (this.message.Body is Multipart)
            {
                var multipart = (Multipart)this.message.Body;
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
            throw new NotImplementedException();
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
