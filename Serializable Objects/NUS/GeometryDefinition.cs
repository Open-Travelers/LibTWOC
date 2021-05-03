using System;
using System.Collections.Generic;
using LibTWOC.Utils;
namespace LibTWOC.Objects
{
    public struct GeometryDefinition : ISerializableData
    {
        public uint MaterialIndex;
        public List<Vertex> Vertices;
        public List<Primitive> Primitives;
        public List<Skin> Skins;
        public List<BlendShape> BlendShapes;
        public GeometryDefinition(uint unknown)
        {
            MaterialIndex = unknown;
            Vertices = new List<Vertex>();
            Primitives = new List<Primitive>();
            Skins = new List<Skin>();
            BlendShapes = new List<BlendShape>();
        }
        public void Read(BinaryReaderExt br)
        {
            //Console.WriteLine("NusGeometryDefinition; Unknown: " + Unknown.ToString("X"));
            MaterialIndex = br.ReadU32();
            //Console.WriteLine($"{Unknown}");
            ReadVertices(br);
            ReadCntrl(br);
            ReadPrimitives(br);
            ReadSkins(br);
            ReadBlendShapes(br);
        }
        private void ReadVertices(BinaryReaderExt br)
        {
            if (Vertices == null) Vertices = new List<Vertex>();
            var vertexCount = br.ReadU32();
            for (var i = 0; i < vertexCount; i++)
            {
                var vtx = new Vertex(0x59);
                vtx.Read(br);
                Vertices.Add(vtx);
            }
        }
        private void ReadCntrl(BinaryReaderExt br)
        {
            var n = br.ReadU32(); // ???
        }
        private void ReadPrimitives(BinaryReaderExt br)
        {
            if (Primitives == null) Primitives = new List<Primitive>();
            var primitiveCount = br.ReadU32();
            //Console.WriteLine("NusGeometryDefinition; Prim count: " + primitiveCount.ToString("X"));
            for (var i = 0; i < primitiveCount; i++)
            {
                var prim = new Primitive();
                prim.Read(br);
                Primitives.Add(prim);
            }
        }
        private void ReadSkins(BinaryReaderExt br)
        {
            if (Skins == null) Skins = new List<Skin>();
            var skinCount = br.ReadU32();
            //Console.WriteLine("NusGeometryDefinition; Skin count: " + skinCount.ToString("X"));
            for (var i = 0; i < skinCount; i++)
            {
                var skin = new Skin((uint)Vertices.Count);
                skin.Read(br);
                Skins.Add(skin);
            }
        }
        private void ReadBlendShapes(BinaryReaderExt br)
        {
            if (BlendShapes == null) BlendShapes = new List<BlendShape>();
            var blendShapeNumber = br.ReadU32();
            //Console.WriteLine("NusGeometryDefinition; Blend shape number: " + blendShapeNumber.ToString("X"));
            if (blendShapeNumber > 0)
            {
                // sorcery
            }
        }

        public void Write(BinaryWriterExt ext)
        {
            throw new NotImplementedException();
        }
    }
}
