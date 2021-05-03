using LibTWOC.Utils;
using LibTWOC.Utils.Math;
using System;

namespace LibTWOC.Objects
{
    public struct AI : ISerializableData
    {
        public string Type;
        public Vec3 Position;
        public AI(string type)
        {
            Type = type;
            Position = Vec3.Zero;
        }
       
        public void Read(BinaryReaderExt br)
        {
            Position.Read(br);
        }

        public void Write(BinaryWriterExt bw)
        {
            throw new NotImplementedException("bruh");
        }
    }
}
