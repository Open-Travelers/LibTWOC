using System;
using System.Collections.Generic;
using System.IO;
using LibTWOC.Utils;
using LibTWOC.Utils.Math;
namespace LibTWOC.Objects
{
    public struct GeometryObject : ISerializableData
    {
        public uint UnknownCount1 { get; private set; }
        public byte[] UnknownData { get; private set; }
        public List<GeometryDefinition> GeometryDefinitions;
        public List<GeometryFace> Faces;
        public void Read(BinaryReaderExt br)
        {
            GeometryDefinitions = new List<GeometryDefinition>();
            Faces = new List<GeometryFace>();
            UnknownCount1 = br.ReadU32();
            for (var i = 0; i < UnknownCount1; i++)
            {
                var IsFace = br.ReadU32();
                if (IsFace == 0)
                {
                    UnknownData = br.ReadBytes(0xC);
                    var geometryDefinitionCount = br.ReadU32();
                    for (var z = 0; z < geometryDefinitionCount; z++)
                    {
                        var definition = new GeometryDefinition();
                        definition.Read(br);
                        GeometryDefinitions.Add(definition);
                    }
                } else if (IsFace == 1) {
                    var faceCount = br.ReadU32();
                    for (var z = 0; z < faceCount; z++)
                    {
                        var face = new GeometryFace();
                        face.Read(br);
                        Faces.Add(face);
                    }
                }
            }
        }

        public void Write(BinaryWriterExt bw)
        {
            throw new NotImplementedException();
        }
        
        public void ExportOBJ(string name)
        {
            int index = 0;
            foreach (var def in GeometryDefinitions)
            {
                var fileName = $"{name}_{index}.obj";
                Console.WriteLine($"Writing PLY '{fileName}'...");
                if (File.Exists(fileName))
                    File.Delete(fileName);
                using (StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Create)))
                {
                    var vs = "";
                    var vns = "";
                    var vts = ""; ;
                    for (var i = 0; i < def.Vertices.Count; i++)
                    {
                        var vtx = def.Vertices[i];
                        vs += $"v {-vtx.Position.X} {vtx.Position.Y} {vtx.Position.Z}\n";
                        vns += $"vn {-vtx.Normal.X} {vtx.Normal.Y} {vtx.Normal.Z}\n";
                        vts += $"vt {-vtx.UV.X} {vtx.UV.Y}\n";
                    }
                    sw.Write(vs);
                    sw.Write(vns);
                    sw.Write(vts);
                    foreach (var prim in def.Primitives)
                    {
                        switch (prim.Type)
                        {
                            case (uint)GeometryPrimitiveType.TriangleStrip: // triangle strip
                                int i = 0;
                                var ts = "";
                                while (i < prim.Data.Count)
                                {
                                    var faceSize = prim.Data[i++];
                                    for (var z = 2; z < faceSize; z++)
                                    {
                                        var a = prim.Data[i + z - 2] + 1;
                                        var b = prim.Data[i + z - 1] + 1;
                                        var c = prim.Data[i + z] + 1;
                                        if (((z - 2) % 2) == 0)
                                            sw.Write($"f {a} {b} {c}\n");
                                        else
                                            sw.Write($"f {a} {c} {b}\n");
                                    }
                                    i += faceSize;
                                }
                                sw.Write(ts);
                                break;
                            case (uint)GeometryPrimitiveType.TriangleList:
                                for (var z = 0; z < prim.Data.Count; z += 3)
                                {
                                    var a = prim.Data[z];
                                    var b = prim.Data[z + 1];
                                    var c = prim.Data[z + 2];
                                    sw.Write($"f {a} {b} {c}\n");
                                }
                                break;
                            default:
                                throw new NotImplementedException($"Export primitive type {prim.Type} not implemented yet.");
                        }
                    }
                }
                index++;
            }
        }
        public void ExportPLY(string name, Mat4x4 mat)
        {
            int index = 0;
            foreach (var def in GeometryDefinitions)
            {

                var fileName = $"{name}_{index}.ply";
                Console.WriteLine($"Writing '{fileName}'...");
                var hdr = "ply\n";
                hdr += "format ascii 1.0\n";
                hdr += $"element vertex {def.Vertices.Count}\n";
                hdr += "property float x\n";
                hdr += "property float y\n";
                hdr += "property float z\n";
                hdr += "property float nx\n";
                hdr += "property float ny\n";
                hdr += "property float nz\n";
                hdr += "property float s\n";
                hdr += "property float t\n";
                hdr += "property uchar red\n";
                hdr += "property uchar green\n";
                hdr += "property uchar blue\n";
                
                using (StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Create)))
                {
                    var vts = "";
                    for (var i = 0; i < def.Vertices.Count; i++)
                    {
                        var vtx = def.Vertices[i];
                        var newPos = Vec3.Transform(vtx.Position, mat);
                        vts += $"{newPos.X} {newPos.Z} {newPos.Y} {vtx.Normal.X} {vtx.Normal.Z} {vtx.Normal.Y} {-vtx.UV.X} {1 - vtx.UV.Y} {vtx.Color & 0x000000FF} {(vtx.Color & 0x0000FF00) >> 8} {(vtx.Color & 0x00FF0000) >> 16}\n";
                    }
                    var faceCount = 0;
                    var fs = "";
                    foreach (var prim in def.Primitives)
                    {
                        switch (prim.Type)
                        {
                            case 6: // triangle strip
                                var tsi = 0;
                                while (tsi < prim.Data.Count)
                                {
                                    var faceSize = prim.Data[tsi++];
                                    for (var z = 2; z < faceSize; z++)
                                    {
                                        var a = prim.Data[tsi + z - 2];
                                        var b = prim.Data[tsi + z - 1];
                                        var c = prim.Data[tsi + z];
                                        if (((z - 2) % 2) == 0)
                                            fs += $"3 {a} {b} {c}\n";
                                        else
                                            fs += $"3 {a} {c} {b}\n";
                                        faceCount++;
                                    }
                                    tsi += faceSize;
                                }
                                break;
                            case 5:
                                for (var i = 0; i < prim.Data.Count; i += 3)
                                {
                                    var a = prim.Data[i];
                                    var b = prim.Data[i + 1];
                                    var c = prim.Data[i + 2];
                                    fs += $"3 {a} {b} {c}\n";
                                    faceCount++;
                                }
                                break;
                            default:
                                throw new NotImplementedException($"Export primitive type {prim.Type} not implemented yet.");
                        }
                    }
                    hdr += $"element face {faceCount}\n";
                    hdr += "property list uchar int vertex_index\n";
                    hdr += "end_header\n";
                    sw.Write(hdr);
                    sw.Write(vts);
                    sw.Write(fs);
                }
                index++;
            }
        }
    }
}
