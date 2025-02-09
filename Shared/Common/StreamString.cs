using System.IO;
using System.Text;

namespace ColorControl.Shared.Common
{
    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public async Task<string> ReadStringAsync()
        {
            var lenBytes = new byte[4];

            var read = await ioStream.ReadAsync(lenBytes, 0, lenBytes.Length);

            if (read < lenBytes.Length)
            {
                return null;
            }

            var len = BitConverter.ToInt32(lenBytes);

            var inBuffer = new byte[len];
            ioStream.ReadExactly(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        public string ReadString()
        {
            var lenBytes = new byte[4];

            var read = ioStream.Read(lenBytes, 0, lenBytes.Length);

            if (read < lenBytes.Length)
            {
                return null;
            }

            var len = BitConverter.ToInt32(lenBytes);

            var inBuffer = new byte[len];
            ioStream.ReadExactly(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        public async Task<int> WriteStringAsync(string outString)
        {
            var outBuffer = streamEncoding.GetBytes(outString);
            var len = outBuffer.Length;

            var lenBytes = BitConverter.GetBytes(len);

            await ioStream.WriteAsync(lenBytes, 0, lenBytes.Length);
            await ioStream.WriteAsync(outBuffer, 0, len);
            await ioStream.FlushAsync();

            return outBuffer.Length + lenBytes.Length;
        }

        public int WriteString(string outString)
        {
            var outBuffer = streamEncoding.GetBytes(outString);
            var len = outBuffer.Length;

            var lenBytes = BitConverter.GetBytes(len);

            ioStream.Write(lenBytes, 0, lenBytes.Length);
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + lenBytes.Length;
        }
    }
}
