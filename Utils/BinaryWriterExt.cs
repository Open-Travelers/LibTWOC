using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibTWOC.Utils
{
    public class BinaryWriterExt 
    {
        public Endianness Endianness = Endianness.Little;
        public Stream BaseStream;

        public BinaryWriterExt(Stream stream, Endianness endianness = Endianness.Little)
        {
            BaseStream = stream;
            Endianness = endianness;
        }

        public void Write(byte[] array, int offset, int count)
        {
            BaseStream.Write(array, offset, count);
        }
        public void WriteU8(byte v)
        {
            BaseStream.WriteByte(v);
        }
        public void WriteString(string temp)
        {
            var bytes = Encoding.ASCII.GetBytes(temp);
            Write(bytes, 0, bytes.Length);
        }
        public void WriteU16(ushort v)
        {
            var array = BitConverter.GetBytes(v);
            if (Endianness == Endianness.Big)
                Array.Reverse(array);
            Write(array, 0, array.Length);
        }
        public void WriteU32(uint v)
        {
            var array = BitConverter.GetBytes(v);
            if (Endianness == Endianness.Big)
                Array.Reverse(array);
            Write(array, 0, array.Length);
        }
        public void WriteF32(float v)
        {
            var array = BitConverter.GetBytes(v);
            if (Endianness == Endianness.Big)
                Array.Reverse(array);
            Write(array, 0, array.Length);
        }

        public void WriteU16(ushort v, Endianness endiannessOverride)
        {
            var array = BitConverter.GetBytes(v);
            if (endiannessOverride == Endianness.Big)
                Array.Reverse(array);
            Write(array, 0, array.Length);
        }
        public void WriteU32(uint v, Endianness endiannessOverride)
        {
            var array = BitConverter.GetBytes(v);
            if (endiannessOverride == Endianness.Big)
                Array.Reverse(array);
            Write(array, 0, array.Length);
        }
        public void WriteF32(float v, Endianness endiannessOverride)
        {
            var array = BitConverter.GetBytes(v);
            if (endiannessOverride == Endianness.Big)
                Array.Reverse(array);
            Write(array, 0, array.Length);
        }


    }
}
