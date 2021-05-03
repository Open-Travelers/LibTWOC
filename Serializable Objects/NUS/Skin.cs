using System;
using LibTWOC.Utils;

namespace LibTWOC.Objects
{
    public struct Skin : ISerializableData
    {
        public uint Unknown0;
        public uint Unknown1;
        public uint Count;
        private uint VertexCount;
        public Skin(uint vertexCount)
        {
            VertexCount = vertexCount;
            Unknown0 = 0;
            Unknown1 = 0;
            Count = 0;
        }
        public void Read(BinaryReaderExt br)
        {
            var skinUnk0 = br.ReadU32();
            if (skinUnk0 != 0)
            {
                var skinUnk1 = br.ReadU8();
                if (skinUnk1 == 0)
                {
                    var skinUnk2 = br.ReadU32();
                    var skinUnk3 = br.ReadU32();
                    var skinUnk4 = br.ReadU32();
                    for (var i = 0; i < skinUnk4; i++)
                        br.ReadU32();
                    br.ReadBytes((int)skinUnk3 * (int)skinUnk4 * 4);
                }
                else {
                    br.ReadBytes((int)VertexCount * 0x24);
                }
            }
        }

        public void Write(BinaryWriterExt bw)
        {
            throw new NotImplementedException();
        }
    }
}
