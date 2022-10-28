using System;
using System.IO;
using System.Text;

namespace ColorControl.Common
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
            ioStream.Read(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
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
