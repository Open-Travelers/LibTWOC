using LibTWOC.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibTWOC.Objects
{
    public enum ConsoleType
    {
        None,
        Gamecube,
        PS2,
        Xbox
    }
    public enum ExecutableType
    {
        Unknown,
        ELF,
        DOL,
        XBE
    }
    public abstract class ExecutableBase : Serializable
    {
        public ConsoleType ConsoleType = ConsoleType.None;
        public ExecutableType Type = ExecutableType.Unknown;
        public abstract int VirtualToPhysical(uint address);
    }

    public class ExecutableGamecubeDol : ExecutableBase
    {
        public struct SectionData
        {
            public uint Offset;
            public uint Size;
            public uint Address;
            public bool Executable;
            public byte[] Data;
        }
        public struct BssData
        {
            public uint Address;
            public uint Size;
        }
        
        public SectionData[] Sections;
        public BssData Bss;
        public uint EntryPoint;

        public ExecutableGamecubeDol()
        {
            ConsoleType = ConsoleType.Gamecube;
            Type = ExecutableType.DOL;
            Sections = new SectionData[18];
            Bss = new BssData();
            for (var i = 0; i < 18; i++)
            {
                Sections[i] = new SectionData();
                if (i < 7)
                    Sections[i].Executable = true;
            }
        }

        protected (int, uint) AddressToSection(uint address)
        {
            var valid = new List<int>();
            for (var i = 0; i < 18; i++)
            {
                if (Sections[i].Offset != 0 && address >= Sections[i].Address && address < Sections[i].Address + Sections[i].Size)
                    valid.Add(i);
            }

            if (valid.Count == 0)
            {
                throw new AccessViolationException($"Virtual address 0x{address:X} not found in executable.");
            }
            else if (valid.Count == 1)
            {
                return (valid[0], address - Sections[valid[0]].Address);
            }
            else if (valid.Count > 1)
            {
                throw new AccessViolationException($"Virtual address 0x{address:X} found in executable multiple times.");
            }
            return (-1, 0);
        }
        public override int VirtualToPhysical(uint address)
        {
            var (section, offset) = AddressToSection(address);
            return (int)(Sections[section].Offset + offset);
        }
        public override void Read(BinaryReaderExt br)
        {
            for (var i = 0; i < 18; i++)
            {
                Sections[i].Offset = br.ReadU32();
            }
            for (var i = 0; i < 18; i++)
            {
                Sections[i].Address = br.ReadU32();
            }
            for (var i = 0; i < 18; i++)
            {
                Sections[i].Size = br.ReadU32();
            }
            Bss.Address = br.ReadU32();
            Bss.Size = br.ReadU32();
            EntryPoint = br.ReadU32();
            for (var i = 0; i < 18; i++)
            {
                if (Sections[i].Offset != 0)
                {
                    //Console.WriteLine($"Size 0x{Sections[i].Size:X}");
                    Sections[i].Data = new byte[Sections[i].Size];
                    br.Seek(Sections[i].Offset, System.IO.SeekOrigin.Begin);
                    br.Read(Sections[i].Data, 0, (int)Sections[i].Size);
                }
            }
        }
        public override void Write(BinaryWriterExt bw)
        {
            for (var i = 0; i < 18; i++)
            {
                bw.WriteU32(Sections[i].Offset);
            }
            for (var i = 0; i < 18; i++)
            {
                bw.WriteU32(Sections[i].Address);
            }
            for (var i = 0; i < 18; i++)
            {
                if (Sections[i].Offset != 0)
                    bw.WriteU32((uint)Sections[i].Data.Length);
            }
            bw.WriteU32(Bss.Address);
            bw.WriteU32(Bss.Size);
            bw.WriteU32(EntryPoint);
            var offsets = new List<int>();
            for (var i = 0; i < 18; i++)
            {
                if (Sections[i].Offset != 0)
                {
                    offsets.Add((int)bw.BaseStream.Position);
                    bw.Write(Sections[i].Data, 0, Sections[i].Data.Length);
                }
            }
            bw.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            for (var i = 0; i < 18; i++)
            {
                bw.WriteU32((uint)offsets[i]);
            }
        }
    }
}
