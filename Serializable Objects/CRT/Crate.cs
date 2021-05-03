using System;
using LibTWOC.Utils;
using LibTWOC.Utils.Math;
namespace LibTWOC.Objects
{
    public struct Crate : ISerializableData
    {
        public CrateGroup Parent;

        public Vec3 Position;
        public float a;
        public Vec3 Rotation;
        public byte b, c, d, e;
        public ushort f, g, h, i, j, k, l;

        private uint Version;
        public Crate(CrateGroup parent, uint version = 4)
        {
            Version = version;
            Parent = parent;
            Position = Vec3.Zero;
            a = 0;
            Rotation = Vec3.Zero;
            b = c = d = e = 0;
            f = g = h = i = j = k = l = 0;
        }
        public void Read(BinaryReaderExt br)
        {
            Position.Read(br);
            a = br.ReadF32();

            var sx = (ushort)(br.ReadU16());
            var sz = (ushort)(br.ReadU16());
            var sy = (ushort)(br.ReadU16());
            Rotation.X = (float)(sx * ((2 * Math.PI) / 65536f));
            Rotation.Y = (float)(sy * ((2 * Math.PI) / 65536f));
            Rotation.Z = (float)(sz * ((2 * Math.PI) / 65536f));

            b = br.ReadU8();
            c = (byte)0xFF;
            d = (byte)0xFF;
            e = (byte)0xFF;
            if (Version >= 3)
            {
                c = br.ReadU8();
                d = br.ReadU8();
                e = br.ReadU8();
            }
            f = br.ReadU16();
            g = br.ReadU16();
            h = br.ReadU16();

            i = br.ReadU16();
            j = br.ReadU16();
            k = br.ReadU16();

            l = (ushort)0xFFFF;
            if (Version >= 3)
            {
                l = br.ReadU16();
            }
        }

        public void Write(BinaryWriterExt bw)
        {
            Position.Write(bw);
            bw.WriteF32(a);

            bw.WriteU16((ushort)Rotation.X);
            bw.WriteU16((ushort)Rotation.Z);
            bw.WriteU16((ushort)Rotation.Y);

            bw.WriteU8(b);
            bw.WriteU8(c);
            bw.WriteU8(d);
            bw.WriteU8(e);

            bw.WriteU16(f);
            bw.WriteU16(g);
            bw.WriteU16(h);

            bw.WriteU16(i);
            bw.WriteU16(j);
            bw.WriteU16(k);
            bw.WriteU16(l);
        }
    }
}
