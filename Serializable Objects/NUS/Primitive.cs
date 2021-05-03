using System;
using System.Collections.Generic;

using LibTWOC.Utils;
namespace LibTWOC.Objects
{
    public enum GeometryPrimitiveType
    {
        TriangleList = 5,
        TriangleStrip = 6
    }
    public struct Primitive : ISerializableData
    {
        public uint Type;
        public List<ushort> Data;
        public void Read(BinaryReaderExt br)
        {
            Data = new List<ushort>();
            Type = br.ReadU32();
            var length = br.ReadU32();
            for (var i = 0; i < length; i++)
                Data.Add(br.ReadU16());
        }

        public void Write(BinaryWriterExt bw)
        {
            throw new NotImplementedException();
        }
    }
}
