using LibTWOC.Utils.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibTWOC.Utils
{
    public enum Endianness
    {
        Little,
        Big
    }
    public enum AutoIncrement
    {
        Automatic, 
        Manual
    }
    public class BinaryReaderExt
    {
        public Endianness Endianness = Endianness.Little;
        public Stream BaseStream;
        public AutoIncrement AutoIncrement = AutoIncrement.Automatic;
        public int Counter; // hack. bad.
        public BinaryReaderExt(Stream stream, Endianness endianness = Endianness.Little)
        {
            BaseStream = stream;
            Endianness = endianness;
        }
        public long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }
        public int Read(byte[] array, int offset, int count)
        {
            var len = BaseStream.Read(array, offset, count);
            if (AutoIncrement == AutoIncrement.Manual)
            {
                Counter -= len;
                BaseStream.Seek(-len, SeekOrigin.Current);
            }
            Counter += len;
            return len;
        }
        public byte[] ReadBytes(int count)
        {
            byte[] array = new byte[count];
            Read(array, 0, count);
            return array;
        }
        public byte ReadU8()
        {
            var result = BaseStream.ReadByte();
            if (result < 0)
                throw new EndOfStreamException();
            Counter++;

            if (AutoIncrement == AutoIncrement.Manual)
            {
                BaseStream.Seek(-1, SeekOrigin.Current);
                Counter--;
            }
            return (byte)result;
        }
        public string ReadString()
        {
            var resultList = new List<byte>();
            while (BaseStream.Position < BaseStream.Length)
            {
                var temp = ReadU8();
                if (temp == 0)
                    break;
                resultList.Add(temp);
            }
            return Encoding.ASCII.GetString(resultList.ToArray());
        }
        public string ReadString(int count)
        {
            var array = ReadBytes(count);
            var length = 1;
            while (length <= count && (array[length - 1] != 0))
            {
                //Console.WriteLine(length);
                length++;
            }
            if (array[length - 1] == 0) length--;
            return Encoding.ASCII.GetString(array, 0, length);
        }
        public ushort ReadU16()
        {
            var array = ReadBytes(2);
            if (Endianness == Endianness.Big)
                Array.Reverse(array);
            return BitConverter.ToUInt16(array, 0);
        }
        public uint ReadU32()
        {
            var array = ReadBytes(4);
            if (Endianness == Endianness.Big)
                Array.Reverse(array);
            return BitConverter.ToUInt32(array, 0);
        }
        public float ReadF32()
        {
            var array = ReadBytes(4);
            if (Endianness == Endianness.Big)
                Array.Reverse(array);
            return BitConverter.ToSingle(array, 0);
        }

        public ushort ReadU16(Endianness endiannessOverride)
        {
            var array = ReadBytes(2);
            if (endiannessOverride == Endianness.Big)
                Array.Reverse(array);
            return BitConverter.ToUInt16(array, 0);
        }
        public uint ReadU32(Endianness endiannessOverride)
        {
            var array = ReadBytes(4);
            if (endiannessOverride == Endianness.Big)
                Array.Reverse(array);
            return BitConverter.ToUInt32(array, 0);
        }
        public float ReadF32(Endianness endiannessOverride)
        {
            var array = ReadBytes(4);
            if (endiannessOverride == Endianness.Big)
                Array.Reverse(array);
            return BitConverter.ToSingle(array, 0);
        }

    }
}
