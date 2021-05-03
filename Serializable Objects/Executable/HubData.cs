using LibTWOC.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibTWOC.Objects
{
    public struct HubData : ISerializableData
    {
        public byte[] Levels;
        public byte[] Unknown;
        
        public void Read(BinaryReaderExt br)
        {
            Levels = br.ReadBytes(6);
            Unknown = br.ReadBytes(6);
        }

        public void Write(BinaryWriterExt bw)
        {
            bw.Write(Levels, 0, 6);
            bw.Write(Unknown, 0, 6);
        }
    }
}
