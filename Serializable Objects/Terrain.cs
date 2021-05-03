using System;
using LibTWOC.Utils;
namespace LibTWOC.Objects
{
    public class Terrain : Serializable
    {
        public override void Read(BinaryReaderExt br)
        {
            var offset = br.ReadU32();
            //br.Seek(offset * 2, SeekOrigin.Begin);
            //var unknown = br.ReadU32() & 0xFFFF;

        }

        public override void Write(BinaryWriterExt bw)
        {
            throw new NotImplementedException();
        }
    }
}
