using LibTWOC.Utils;
using LibTWOC.Utils.Math;
namespace LibTWOC.Objects
{
    public struct Wumpa : ISerializableData
    {
        public Vec3 Position;
        public void Read(BinaryReaderExt br)
        {
            Position.Read(br);
        }

        public void Write(BinaryWriterExt bw)
        {
            Position.Write(bw);
        }
    }
}
