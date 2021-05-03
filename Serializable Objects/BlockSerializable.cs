using System;
using System.Collections.Generic;
using System.Text;

using LibTWOC.Utils;
namespace LibTWOC.Collections
{
    public struct BlockInfo
    {
        public uint Type;
        public uint Size;
        public override string ToString()
        {
            return $"BlockInfo {{ Type: 0x{Type:X}; Size: {Size} }}";
        }
    }
    public abstract class BlockSerializable : Serializable
    {
        protected enum LoopResult
        {
            Continue,
            Done,
            Error
        }
        public List<BlockInfo> Blocks = new List<BlockInfo>();
        protected int BlockDepth = -1;
        protected BlockInfo GetCurrentBlock()
        {
            return Blocks[Blocks.Count - 1];
        }
        protected delegate LoopResult BlockFilter(BinaryReaderExt br);
        protected void ReadBlock(BinaryReaderExt br, BlockFilter filter)
        {
            BlockDepth++;
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                ReadBlockHeader(br);
                if (filter(br) != LoopResult.Continue) break;
            }
            BlockDepth--;
        }
        protected void ReadBlockHeader(BinaryReaderExt br)
        {
            var block = new BlockInfo();
            block.Type = br.ReadU32();
            block.Size = br.ReadU32();
            br.Counter = 0;
            Blocks.Add(block);
        }
        protected void PaddingCorrection(BinaryReaderExt br)
        {
            //Console.WriteLine((int)GetCurrentBlock().Size - br.Counter - 8);
            br.ReadBytes((int)GetCurrentBlock().Size - br.Counter - 8);
        }
    }

}
