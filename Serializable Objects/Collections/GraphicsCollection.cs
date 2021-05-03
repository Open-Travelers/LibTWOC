using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibTWOC.Objects;
using LibTWOC.Utils;
using LibTWOC.Utils.Math;
namespace LibTWOC.Collections
{
    public enum GraphicsCollectionType
    {
        Invalid,
        NUS,
        HGO
    }
    public class GraphicsCollection : BlockSerializable
    {
        public GraphicsCollectionType Type = GraphicsCollectionType.Invalid;
        public List<string> Nametable = new List<string>();
        public List<Texture> TextureSet = new List<Texture>();
        public List<Material> MaterialSet = new List<Material>();
        public List<GeometryObject> GeometryObjectSet = new List<GeometryObject>();
        public List<Instance> InstanceData1 = new List<Instance>();
        public List<byte[]> InstanceData2 = new List<byte[]>();

        private const uint BLK_HGOF = 0x464F4748;
        private const uint BLK_HGO0 = 0x304F4748; // BLK_GST equivalent for HGO files

        private const uint BLK_GSC = 0x30435347;
        private const uint BLK_NTBL = 0x4C42544E;
        private const uint BLK_TST = 0x30545354;
        private const uint BLK_TSH = 0x30485354;
        private const uint BLK_TXM = 0x304D5854;
        private const uint BLK_MS = 0x3030534D;
        private const uint BLK_GST = 0x30545347;
        private const uint BLK_INST = 0x54534E49;
        private const uint BLK_SPEC = 0x43455053;
        private const uint BLK_SST = 0x30545353;
        private const uint BLK_TAS = 0x30534154;
        private const uint BLK_ALIB = 0x42494C41;
        private const uint BLK_LDIR = 0x5249444C;
        private const uint BLK_SPHE = 0x45485053;

        private int TextureCount = 0;
        private LoopResult BlockFilterTST(BinaryReaderExt br)
        {
            var block = GetCurrentBlock();
            switch (block.Type)
            {
                case BLK_TSH:
                    TextureCount = (int)br.ReadU32();
                    if (TextureCount == 0)
                        return LoopResult.Done;
                    break;
                case BLK_TXM:
                    Texture texture = new Texture();
                    texture.Read(br);
                    TextureSet.Add(texture);

                    //Console.WriteLine($"info: blk TXM0; W: {texture.Width} H: {texture.Height} T: {texture.Type} DL: {texture.PixelData?.Length} PL: {texture.PaletteData?.Length}");
                    //texture.ToImage().Save(new FileStream($"{TextureSet.Count - 1}.png", FileMode.OpenOrCreate), new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                    TextureCount--;
                    if (TextureCount <= 0)
                        return LoopResult.Done;
                    break;
                default:
                    Console.WriteLine($"error in texture set filter at depth {BlockDepth}, offset {br.BaseStream.Position}: nus load failed; invalid blk header {block.Type:X}");
                    return LoopResult.Error;
            }
            return LoopResult.Continue;
        }
        private LoopResult BlockFilterGSC(BinaryReaderExt br)
        {
            var block = GetCurrentBlock();
            switch (block.Type)
            {
                case BLK_HGOF:
                    if (Type != GraphicsCollectionType.Invalid)
                    {
                        Console.WriteLine($"error in gsc filter at depth {BlockDepth}, offset {br.BaseStream.Position}: graphics archive load failed; file header block found twice");
                        return LoopResult.Error;
                    }

                    Type = GraphicsCollectionType.HGO;
                    if (block.Size != br.BaseStream.Length)
                    {
                        Console.WriteLine($"warning: block-file size mismatch; block info {block}, stream length {br.BaseStream.Length}");
                    }
                    break;
                case BLK_GSC:
                    if (Type != GraphicsCollectionType.Invalid)
                    {
                        Console.WriteLine($"error in gsc filter at depth {BlockDepth}, offset {br.BaseStream.Position}: graphics archive load failed; file header block found twice");
                        return LoopResult.Error;
                    }
                    
                    Type = GraphicsCollectionType.NUS;
                    //Console.WriteLine("info: blk GSC0");
                    // I would very much like to cause an error to occur when the file size is smaller than the GSC block size,
                    // but sadly, SOME game developers didn't think that was a problem, and so a few in-game nus files just have that as a thing.
                    // I don't know why. tool issues perhaps.

                    if (block.Size != br.BaseStream.Length)
                    {
                        Console.WriteLine($"warning: block-file size mismatch; block info {block}, stream length {br.BaseStream.Length}");
                    }
                    break;

                case BLK_NTBL:
                    //Console.WriteLine("info: blk NTBL");
                    var ntblDataLength = br.ReadU32();
                    var ntblByteIndex = 0;
                    var tempByteStorage = new List<byte>();
                    while (ntblByteIndex < ntblDataLength)
                    {
                        var temp = br.ReadU8();
                        if (temp == 0)
                        {
                            if (tempByteStorage.Count > 0)
                            {
                                Nametable.Add(ASCIIEncoding.ASCII.GetString(tempByteStorage.ToArray()));
                                tempByteStorage.Clear();
                            }
                        }
                        else
                        {
                            tempByteStorage.Add(temp);
                        }

                        ntblByteIndex++;
                        if (br.BaseStream.Position >= br.BaseStream.Length)
                        {
                            Console.WriteLine("error: nus load failed; reached eof during NTBL");
                            return LoopResult.Error;
                        }
                    }
                    PaddingCorrection(br);
                    break;

                case BLK_TST:
                    //Console.WriteLine("info: blk TST0");
                    ReadBlock(br, BlockFilterTST);
                    break;
                case BLK_MS:
                    //Console.WriteLine("info: blk MS00");
                    var msCount = br.ReadU32();
                    for (var i = 0; i < msCount; i++)
                    {
                        Material material = new Material();
                        material.Read(br);
                        MaterialSet.Add(material);
                    }
                    PaddingCorrection(br);
                    break;

                case BLK_GST:
                    var gobjCount = br.ReadU32();
                    //Console.WriteLine("info: blk GST0; Count: " + gobjCount.ToString());
                    for (var i = 0; i < gobjCount; i++)
                    {
                        GeometryObject geom = new GeometryObject();
                        geom.Read(br);
                        GeometryObjectSet.Add(geom);
                    }
                    PaddingCorrection(br);
                    break;
                case BLK_HGO0:
                    Console.WriteLine("HGO data block");
                    var hgoUnk0 = br.ReadU8();
                    Console.WriteLine($"first byte is {hgoUnk0}");
                    if (hgoUnk0 != 0)
                    {
                        for (var i = 0; i < hgoUnk0; i++)
                        {
                            br.ReadU8();
                            br.ReadBytes(0x40);
                            br.ReadBytes(0x40);
                            br.ReadBytes(0x40);
                            br.ReadBytes(0xC);
                            br.ReadU8();
                            br.ReadU32();
                        }
                        var hgoUnk1 = br.ReadU8();
                        br.ReadBytes(hgoUnk1);
                        Console.WriteLine($"second value is {hgoUnk1}");
                        
                        var hgoUnk2 = br.ReadU8();
                        br.ReadBytes(hgoUnk2);
                        Console.WriteLine($"third value is {hgoUnk2}");

                        var hgoUnk3 = br.ReadU8();
                        Console.WriteLine($"fourth value is {hgoUnk3}");
                        for (var i = 0; i < hgoUnk3; i++)
                        {
                            Console.WriteLine($"loop {i}");
                            var hgoUnk4 = br.ReadU32();
                            Console.WriteLine($"unk4 {hgoUnk4}");
                            var hgoUnk5 = br.ReadU8();
                            Console.WriteLine($"unk5 {hgoUnk5}");
                            if (hgoUnk5 != 0)
                            {
                                for (var z = 0; z < hgoUnk0; z++)
                                {
                                    var hgoUnk6 = br.ReadU8();
                                    Console.WriteLine($"subloop {z}, value {hgoUnk6}");
                                    if (hgoUnk6 != 0)
                                    {
                                        GeometryObject geom = new GeometryObject();
                                        geom.Read(br);
                                        GeometryObjectSet.Add(geom);
                                    }
                                }
                            }
                            
                            var hgoUnk7 = br.ReadU8();
                            Console.WriteLine($"unk7 {hgoUnk7}");
                            if (hgoUnk7 != 0)
                            {
                                GeometryObject geom = new GeometryObject();
                                geom.Read(br);
                                GeometryObjectSet.Add(geom);
                            }
                            
                            var hgoUnk8 = br.ReadU8();
                            Console.WriteLine($"unk8 {hgoUnk8}");
                            if (hgoUnk8 != 0)
                            {
                                for (var z = 0; z < hgoUnk0; z++)
                                {
                                    var hgoUnk9 = br.ReadU8();
                                    Console.WriteLine($"subloop {z}: value {hgoUnk9}");
                                    if (hgoUnk9 != 0)
                                    {
                                        GeometryObject geom = new GeometryObject();
                                        geom.Read(br);
                                        GeometryObjectSet.Add(geom);
                                    }
                                }
                            }

                            var hgoUnk10 = br.ReadU8();
                            Console.WriteLine($"unk10 {hgoUnk10}");
                            if (hgoUnk10 != 0)
                            {
                                GeometryObject geom = new GeometryObject();
                                geom.Read(br);
                                GeometryObjectSet.Add(geom);
                            }
                        }
                        var hgoUnk11 = br.ReadU8();
                        Console.WriteLine($"uhhh value is {hgoUnk11}");
                        for (var i = 0; i < hgoUnk11; i++)
                        {
                            br.ReadU8();
                            br.ReadBytes(0x40);
                            br.ReadU32();
                        }
                        var hgoUnk12 = br.ReadU8();
                        Console.WriteLine($"whoa value is {hgoUnk12}");
                        for (var i = 0; i < hgoUnk12; i++)
                        {
                            var hgoUnk13 = br.ReadU8();
                            br.ReadBytes(hgoUnk13 * 0x30);

                            var hgoUnk14 = br.ReadU8();
                            br.ReadBytes(hgoUnk14 * 0x60);

                            var hgoUnk15 = br.ReadU8();
                            Console.WriteLine($"bruh value is {hgoUnk15}");
                            for (var z = 0; z < hgoUnk15; z++)
                            {
                                var hgoUnk16 = br.ReadU32();
                                br.ReadBytes((int)hgoUnk16 * 16);
                                var hgoUnk17 = br.ReadU32();
                                br.ReadBytes((int)hgoUnk17 * 16);
                            }
                            br.ReadU8();
                        }
                    }
                    Console.Write("Weird floats: ");
                    for (var i = 0; i < 11; i++)
                        Console.WriteLine($"{br.ReadF32()}; ");

                    PaddingCorrection(br);
                    break;
                case BLK_INST:
                    var instCount1 = br.ReadU32();

                    for (var i = 0; i < instCount1; i++)
                    {
                        var inst = new Instance();
                        inst.Read(br);
                        InstanceData1.Add(inst);
                        //Console.WriteLine($"Inst Index {inst.ModelIndex}");
                    }

                    var instCount2 = br.ReadU32();
                    for (var i = 0; i < instCount2; i++) InstanceData2.Add(br.ReadBytes(0x60));
                    //Console.WriteLine($"info: blk INST; L1: {instCount1} L2: {instCount2}");
                    break;
                case BLK_SPEC:
                    //Console.WriteLine("info: blk SPEC; TODO");
                    var specObjectCount = br.ReadU32();
                    var specObjectData = br.ReadBytes((int)specObjectCount * 0x50);
                    for (var i = 0; i < specObjectCount; i++)
                    {
                        // ??? pointer black magic
                    }
                    break;
                case BLK_SST:
                    Console.WriteLine("info: blk SST0");
                    var splineCount = br.ReadU32();
                    var splineDataLength = br.ReadU32();
                    var splineData = br.ReadBytes((int)splineDataLength);
                    for (var i = 0; i < splineDataLength; i++)
                    {
                        Console.Write($"{splineData[i]:X2} ");
                    }
                    break;
                case BLK_TAS:
                    Console.WriteLine("info: blk TAS0; TODO");
                    br.ReadBytes((int)block.Size - 8);
                    break;
                case BLK_ALIB:
                    Console.WriteLine("info: blk ALIB; TODO");
                    br.ReadBytes((int)block.Size - 8);
                    break;
                case BLK_LDIR:
                    Console.WriteLine("info: blk LDIR; TODO");
                    br.ReadBytes((int)block.Size - 8);
                    break;
                case BLK_SPHE:
                    Console.WriteLine("info: blk SPHE; TODO");
                    br.ReadBytes((int)block.Size - 8);
                    break;
                default:
                    Console.WriteLine($"error in gsc filter at depth {BlockDepth}, offset {br.BaseStream.Position}: nus load failed; invalid blk header {block.Type:X}");
                    return LoopResult.Error;
            }
            return LoopResult.Continue;
        }

        public override void Read(BinaryReaderExt br)
        {
            ReadBlock(br, BlockFilterGSC);
        }

        public override void Write(BinaryWriterExt bw)
        {
            throw new NotImplementedException();
        }
    }
}
