using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace LibTWOC
{
    public static class Utilities
    {
        #region Binary Data Utilities
        public static uint SwapU32(uint val)
        {
            var intAsBytes = BitConverter.GetBytes(val);
            Array.Reverse(intAsBytes);
            return BitConverter.ToUInt32(intAsBytes, 0);
        }
        public static ushort SwapU16(ushort val)
        {
            var intAsBytes = BitConverter.GetBytes(val);
            Array.Reverse(intAsBytes);
            return BitConverter.ToUInt16(intAsBytes, 0);
        }
        public static ushort ArrayReadU16(byte[] array, long offset, bool bigEndian = true)
        {
            if (bigEndian)
                return (ushort)(array[offset + 1] | (array[offset] << 8));
            return (ushort)(array[offset] | (array[offset + 1] << 8));
        }
        public static uint ArrayReadU32(byte[] array, long offset, bool bigEndian = true)
        {
            if (bigEndian)
                return (uint)(array[offset + 3] | (array[offset + 2] << 8) | (array[offset + 1] << 16) | (array[offset] << 24));
            return (uint)(array[offset] | (array[offset + 1] << 8) | (array[offset + 2] << 16) | (array[offset + 3] << 24));
        }
        #endregion
        #region DXT Compression Utilities: Adapted from Dolphin source code

        public static int DXTBlend(int v1, int v2)
        {
            return ((v1 * 3 + v2 * 5) >> 3);
        }
        public static byte Convert3To8(byte v)
        {
            return (byte)((v << 5) | (v << 2) | (v >> 1));
        }
        public static byte Convert4To8(byte v)
        {
            return (byte)((v << 4) | v);
        }
        public static byte Convert5To8(byte v)
        {
            return (byte)((v << 3) | (v >> 2));
        }
        public static byte Convert6To8(byte v)
        {
            return (byte)((v << 2) | (v >> 4));
        }

        public static void DecodeRGB5A3Block(ref Image<Byte4> dst, byte[] src, int srcOffset, int x, int y)
        {
            if (srcOffset >= src.Length) return;
            var c = Utilities.ArrayReadU16(src, srcOffset, true);

            byte r, g, b, a;
            if ((c & 0x8000) != 0)
            {
                r = Utilities.Convert5To8((byte)((c >> 10) & 0x1F));
                g = Utilities.Convert5To8((byte)((c >> 5) & 0x1F));
                b = Utilities.Convert5To8((byte)((c) & 0x1F));
                a = 0xFF;
            }
            else
            {
                a = Utilities.Convert3To8((byte)((c >> 12) & 0x7));
                b = Utilities.Convert4To8((byte)((c >> 8) & 0xF));
                g = Utilities.Convert4To8((byte)((c >> 4) & 0xF));
                r = Utilities.Convert4To8((byte)((c) & 0xF));
            }

            dst[x, y] = new Byte4(r, g, b, a);
        }
        public static void DecodeDXTBlock(ref Image<Byte4> dst, byte[] src, int srcOffset, int blockX, int blockY)
        {
            if (srcOffset >= src.Length) return;
            var c1 = Utilities.ArrayReadU16(src, srcOffset, true);
            var c2 = Utilities.ArrayReadU16(src, srcOffset + 2, true);
            var lines = new byte[4] { src[srcOffset + 4], src[srcOffset + 5], src[srcOffset + 6], src[srcOffset + 7] };
            Console.WriteLine($"4: [{lines[0]}, {lines[1]}, {lines[2]}, {lines[3]}]");
            byte blue1 = Utilities.Convert5To8((byte)(c1 & 0x1F));
            byte blue2 = Utilities.Convert5To8((byte)(c2 & 0x1F));
            byte green1 = Utilities.Convert6To8((byte)((c1 >> 5) & 0x3F));
            byte green2 = Utilities.Convert6To8((byte)((c2 >> 5) & 0x3F));
            byte red1 = Utilities.Convert5To8((byte)((c1 >> 11) & 0x1F));
            byte red2 = Utilities.Convert5To8((byte)((c2 >> 11) & 0x1F));

            Byte4[] colors = new Byte4[4];
            colors[0] = new Byte4(red1, green1, blue1, 255);
            colors[1] = new Byte4(red2, green2, blue2, 255);
            if (c1 > c2)
            {
                colors[2] = new Byte4((byte)Utilities.DXTBlend(red2, red1), (byte)Utilities.DXTBlend(green2, green1), (byte)Utilities.DXTBlend(blue2, blue1), 255);
                colors[3] = new Byte4((byte)Utilities.DXTBlend(red1, red2), (byte)Utilities.DXTBlend(green1, green2), (byte)Utilities.DXTBlend(blue1, blue2), 255);
            }
            else
            {
                // color[3] is the same as color[2] (average of both colors), but transparent.
                // This differs from DXT1 where color[3] is transparent black.
                colors[2] = new Byte4((byte)((red1 + red2) / 2), (byte)((green1 + green2) / 2), (byte)((blue1 + blue2) / 2), 255);
                colors[3] = new Byte4((byte)((red1 + red2) / 2), (byte)((green1 + green2) / 2), (byte)((blue1 + blue2) / 2), 0);
            }

            for (int y = 0; y < 4; y++)
            {
                int val = lines[y];
                for (int x = 0; x < 4; x++)
                {
                    dst[x + blockX, y + blockY] = colors[(val >> 6) & 3];
                    val <<= 2;
                }
            }
        }
        #endregion
    }
}
