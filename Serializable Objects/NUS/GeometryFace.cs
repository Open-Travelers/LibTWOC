using System;
using LibTWOC.Utils;
namespace LibTWOC.Objects
{
    public struct GeometryFace : ISerializableData
    {
        public uint a;
        public uint Count;
        public uint b;
        public float c;
        public byte[] Data;

        public void Read(BinaryReaderExt br)
        {
            a = br.ReadU32();
            Count = br.ReadU32();
            b = br.ReadU32();
            c = br.ReadF32();

            Data = br.ReadBytes((int)Count * 0x18);
        }

        public void Write(BinaryWriterExt bw)
        {
            throw new NotImplementedException();
        }
    }

}
