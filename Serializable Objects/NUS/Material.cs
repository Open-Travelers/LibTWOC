using System;
using LibTWOC.Utils;
namespace LibTWOC.Objects
{
    public struct Material : ISerializableData
    {
        public int Texture;
        public byte[] UnknownData1, UnknownData2, UnknownData3;
        public float DiffuseR, DiffuseG, DiffuseB, Unknown4, Unknown5;
        public override string ToString()
        {
            var s = $"Material {{ \nTextureID: {Texture}; \nDiffuse R: {DiffuseR}; \nDiffuse G: {DiffuseG}; \nDiffuse B: {DiffuseB}; \nUnkFloat4: {Unknown4}; \nUnkFloat5: {Unknown5};";
            s += "\nUnkData1: [";
            foreach (var b in UnknownData1)
                s += b.ToString("X2") + "; ";
            s += "]; \nUnkData2: [";
            foreach (var b in UnknownData2)
                s += b.ToString("X2") + "; ";
            s += "]; \nUnkData3: [";
            foreach (var b in UnknownData3)
                s += b.ToString("X2") + "; ";
            s += "];\n}";
            return s;
        }
        public void Read(BinaryReaderExt br)
        {
            UnknownData1 = br.ReadBytes(20);
            DiffuseR = br.ReadF32();
            DiffuseG = br.ReadF32();
            DiffuseB = br.ReadF32();
            UnknownData2 = br.ReadBytes(16);
            Unknown4 = br.ReadF32();
            Unknown5 = br.ReadF32();

            Texture = (int)br.ReadU32();
            UnknownData3 = br.ReadBytes(24);
        }

        public void Write(BinaryWriterExt bw)
        {
            throw new NotImplementedException();
        }
    }
}
