using System.IO;

namespace System.Security.Cryptography
{
    /// <summary>
    /// HashAlgorithm 扩展
    /// </summary>
    public static class HashAlgorithmExtension
    {
        /// <summary>
        /// 计算指定 <see cref="Stream"/> 对象的哈希值。
        /// </summary>
        /// <param name="hashAlgorithm">要计算的哈希算法</param>
        /// <param name="inputStream">要计算其哈希代码的输入</param>
        /// <param name="streamReadAction">没次读流时调用，例如用来获取进度</param>
        /// <returns>计算所得的哈希代码</returns>
        public static byte[] ComputeHash(this HashAlgorithm hashAlgorithm, Stream inputStream, Action streamReadAction)
        {
            using (var readStream = new ReadStream(inputStream, streamReadAction))
                return hashAlgorithm.ComputeHash(readStream);
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
