using System;
using LibTWOC.Utils;
using LibTWOC.Utils.Math;
namespace LibTWOC.Objects
{
    public struct Instance : ISerializableData
    {
        public Mat4x4 Transform;
        public uint ModelIndex;
        public byte[] UnknownData;
        public override string ToString()
        {
            var s = $"Instance {{\n  Model Index: {ModelIndex}\n  Transform: {Transform}\n  Unknown Data: ";
            foreach (var b in UnknownData)
                s += "0x" + b.ToString("X") + " ";
            return s + "\n}";
        }
        public void Read(BinaryReaderExt br)
        {
            UnknownData = new byte[12];

            Transform.Read(br);
            ModelIndex = br.ReadU32();
            UnknownData = br.ReadBytes(12);
            /*Console.WriteLine("Instance data");
            for (var i = 0; i < 4; i++)
            {
                for (var z = 0; z < 4; z++)
                {
                    Console.Write($"{Transform.Data[z + i * 4]} ");
                }
                Console.WriteLine();
            }*/
        }

        public void Write(BinaryWriterExt bw)
        {
            throw new NotImplementedException();
        }
    }
}
