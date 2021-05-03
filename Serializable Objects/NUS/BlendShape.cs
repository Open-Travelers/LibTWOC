using System;
using LibTWOC.Utils;
namespace LibTWOC.Objects
{
    public struct BlendShape : ISerializableData
    {
        public void Read(BinaryReaderExt br)
        {
            throw new NotImplementedException("Blend shapes not yet implemented.");
        }

        public void Write(BinaryWriterExt bw)
        {
            throw new NotImplementedException();
        }
    }
}
