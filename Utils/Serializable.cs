using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LibTWOC.Utils;
namespace LibTWOC
{
    public interface ISerializableData
    {
        void Read(BinaryReaderExt ext);
        void Write(BinaryWriterExt ext);
    }
    public abstract class Serializable : ISerializableData
    {
        public void FromStream(Stream stream, Endianness endianness)
        {
            var br = new BinaryReaderExt(stream, endianness);
            Read(br);
        }
        public void ToStream(Stream stream, Endianness endianness)
        {
            var bw = new BinaryWriterExt(stream, endianness);
            Write(bw);
        }
        public abstract void Read(BinaryReaderExt br);
        public abstract void Write(BinaryWriterExt bw);
    }
}
