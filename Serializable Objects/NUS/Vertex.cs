using System;
using LibTWOC.Utils;
using LibTWOC.Utils.Math;
namespace LibTWOC.Objects
{
    public struct Vertex : ISerializableData
    {
        public int Type;
        public Vec3 Position;
        public Vec3 Normal;
        public Vec2 UV;
        public uint Color;
        public Vertex(Vertex vertex)
        {
            Type = vertex.Type;
            Position = vertex.Position;
            Normal = vertex.Normal;
            UV = vertex.UV;
            Color = vertex.Color;
        }
        public Vertex(int type)
        {
            Type = type;
            Position = Vec3.Zero;
            Normal = Vec3.Zero;
            UV = Vec2.Zero;
            Color = 0xFFFF00FF;
        }
        public override string ToString()
        {
            return $"Vertex {{Type: 0x{Type:X}; Position: {Position}; Normal: {Normal}; UV: {UV}; Color: 0x{Color:X} }}";
        }
        public void Read(BinaryReaderExt br)
        {
            switch (Type)
            {
                case 0x59:
                    Position.Read(br);
                    Normal.Read(br);
                    Color = br.ReadU32();
                    UV.Read(br);
                    break;
                default:
                    throw new NotImplementedException($"Vertex type 0x{Type:X} not yet implemented.");
            }
        }
        public int ByteLength()
        {
            return Type switch
            {
                0x11 => 0x10,
                0x53 => 0x1C,
                0x51 => 0x18,
                0x59 => 0x24,
                0x5D => 0x38,
                _ => 0,
            };
        }

        public void Write(BinaryWriterExt bw)
        {
            throw new NotImplementedException();
        }
    }
}
