using System;
/// TODO: Figure out if this is within the library's scope or if conversion to a normal texture format should be left to the user
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using LibTWOC.Utils;
namespace LibTWOC.Objects
{
    public struct Texture : ISerializableData
    {
        public uint Type;
        public uint Width;
        public uint Height;

        public byte[] PixelData;
        public byte[] PaletteData;
        public override string ToString()
        {
            return $"Type {Type}, Width: {Width}, Height: {Height}";
        }
        public void Read(BinaryReaderExt br)
        {
            Type = br.ReadU32();
            Width = br.ReadU32();
            Height = br.ReadU32();
            var pixelLength = br.ReadU32();
            if ((Type & 0x80) == 0)
            {
                pixelLength = BaseTextureLength;
            }

            PixelData = new byte[pixelLength];
            br.Read(PixelData, 0, (int)pixelLength);
            var paletteLength = PaletteLength;
            if (paletteLength > 0)
            {
                PaletteData = new byte[paletteLength];
                br.Read(PaletteData, 0, (int)paletteLength);
            }
        }
        public void Write(BinaryWriterExt bw)
        {
            throw new NotImplementedException();
        }

        public Image<Byte4> ToImage()
        {
            var image = new Image<Byte4>((int)Width, (int)Height);
            
            int index;
            switch (Type)
            {
                case 128:
                    index = 0;
                    for (int y = 0; y < Height; y += 8)
                    {
                        for (var x = 0; x < Width; x += 8)
                        {
                            Utilities.DecodeDXTBlock(ref image, PixelData, index, x, y);
                            index += 8;
                            Utilities.DecodeDXTBlock(ref image, PixelData, index, x + 4, y);
                            index += 8;
                            Utilities.DecodeDXTBlock(ref image, PixelData, index, x, y + 4);
                            index += 8;
                            Utilities.DecodeDXTBlock(ref image, PixelData, index, x + 4, y + 4);
                            index += 8;
                        }
                    }
                    break;
                case 129:
                    index = 0;
                    for (var y = 0; y < Height; y += 4)
                    {
                        for (var x = 0; x < Width; x += 4)
                        {
                            for (var by = 0; by < 4; by++)
                            {
                                for (var bx = 0; bx < 4; bx++)
                                {
                                    Utilities.DecodeRGB5A3Block(ref image, PixelData, index, x + bx, y + by);
                                    index += 2;
                                }
                            }
                        }
                    }
                    break;
                case 0x82: // This is incorrect but I just wanted something to render
                    index = 0;
                    for (var y = 0; y < Height; y += 4)
                    {
                        for (var x = 0; x < Width; x += 4)
                        {
                            for (var by = 0; by < 4; by++)
                            {
                                for (var bx = 0; bx < 4; bx++)
                                {
                                    Utilities.DecodeRGB5A3Block(ref image, PixelData, index, x + bx, y + by);
                                    index += 2;
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException($"Texture type 0x{Type:X} not implemented.");
            }
            return image;
        }
        public uint PixelSize
        {
            get
            {
                return Type switch
                {
                    (uint)0 => (uint)16,
                    (uint)1 => (uint)16,
                    (uint)2 => (uint)24,
                    (uint)3 => (uint)32,
                    (uint)4 => (uint)4,
                    (uint)5 => (uint)8,
                    _ => (uint)0,
                };
            }
        }
        
        public uint BaseTextureLength
        {
            get
            {
                uint size = PixelSize * Width * Height;
                if (size < 0)
                {
                    size += 7;
                }
                return size >> 3;
            }
        }
        public uint PaletteLength
        {
            get 
            {
                return Type switch
                {
                    (uint)4 => (uint)0x40,
                    (uint)5 => (uint)0x400,
                    _ => (uint)0,
                };
            }
        }
    }
}
