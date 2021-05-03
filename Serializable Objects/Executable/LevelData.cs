using LibTWOC.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using LibTWOC.Utils.Math;

namespace LibTWOC.Objects
{
    public struct LevelData : ISerializableData
    {
        public uint FilepathPtr;
        public string Filepath;

        public uint ModelListPtr;
        public byte[] Unknown0x8;
        public Vec3 StartPosition;
        public float[] Unknown0x38;
        public byte[] Unknown0x4C;

        public void Read(BinaryReaderExt ext)
        {
            FilepathPtr = ext.ReadU32();
            ModelListPtr = ext.ReadU32();

            Unknown0x8 = ext.ReadBytes(36);
            
            StartPosition = new Vec3(0, 0, 0);
            StartPosition.Read(ext);

            Unknown0x38 = new float[5];
            for (var i = 0; i < 5; i++)
                Unknown0x38[i] = ext.ReadF32();
            
            Unknown0x4C = ext.ReadBytes(12);

        }

        public void Write(BinaryWriterExt ext)
        {
            ext.WriteU32(FilepathPtr);
            ext.WriteU32(ModelListPtr);
            foreach (var b in Unknown0x8)
                ext.WriteU8(b);
            StartPosition.Write(ext);
            
            foreach (var f in Unknown0x38)
                ext.WriteF32(f);

            foreach (var b in Unknown0x4C)
                ext.WriteU8(b);

        }
    }
}
