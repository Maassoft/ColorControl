using System.IO;
using System.Net;

namespace novideo_srgb
{
    public class ICCBinaryReader : BinaryReader
    {
        public ICCBinaryReader(Stream stream) : base(stream)
        {
        }

        public override short ReadInt16()
        {
            return IPAddress.NetworkToHostOrder(base.ReadInt16());
        }

        public override ushort ReadUInt16()
        {
            return (ushort)ReadInt16();
        }

        public override int ReadInt32()
        {
            return IPAddress.NetworkToHostOrder(base.ReadInt32());
        }

        public override uint ReadUInt32()
        {
            return (uint)ReadInt32();
        }

        public override long ReadInt64()
        {
            return IPAddress.NetworkToHostOrder(base.ReadInt64());
        }

        public override ulong ReadUInt64()
        {
            return (ulong)ReadInt64();
        }

        public float ReadU8Fixed8()
        {
            return ReadUInt16() / 256f;
        }

        public double ReadS15Fixed16()
        {
            return ReadInt32() / 65536d;
        }

        public double ReadCIEXYZ()
        {
            return ReadUInt16() / 32768d;
        }
    }
}