using System.IO;

namespace System.Security.Cryptography
{
    public static class HashAlgorithmExtension
    {
        public static byte[] ComputeHash(this HashAlgorithm hashAlgorithm, Stream inputStream, Action action)
        {
            using (var ps = new ReadStream(inputStream, action))
                return hashAlgorithm.ComputeHash(ps);
        }

        class ReadStream : Stream
        {
            Stream stream;
            readonly Action action;

            public ReadStream(Stream stream, Action action)
            {
                this.stream = stream;
                this.action = action;
            }

            public override bool CanRead => stream.CanRead;

            public override bool CanSeek => stream.CanSeek;

            public override bool CanWrite => stream.CanWrite;

            public override long Length => stream.Length;

            public override long Position { get => stream.Position; set => stream.Position = value; }

            public override void Flush() => stream.Flush();

            public override int Read(byte[] buffer, int offset, int count)
            {
                action();
                return stream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin) => stream.Seek(offset, origin);

            public override void SetLength(long value) => stream.SetLength(value);

            public override void Write(byte[] buffer, int offset, int count) => stream.Write(buffer, offset, count);
        }
    }
}
