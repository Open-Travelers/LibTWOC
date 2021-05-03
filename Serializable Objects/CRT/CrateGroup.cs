using System.Collections;
using System.Collections.Generic;
using LibTWOC.Utils;
using LibTWOC.Utils.Math;
namespace LibTWOC.Objects
{
    public struct CrateGroup : ISerializableData, IEnumerable<Crate>
    {
        public List<Crate> Crates;
        public Vec3 Position;
        public ushort CrateOffset;
        public ushort Tilt;
        private uint Version;
        public CrateGroup(uint version = 4)
        {
            Version = version;
            Crates = new List<Crate>();
            Position = Vec3.Zero;
            CrateOffset = 0;
            Tilt = 0;

        }

        public void Read(BinaryReaderExt br)
        {
            Position.Read(br);

            CrateOffset = br.ReadU16();
            var CrateCount = br.ReadU16();
            Tilt = br.ReadU16();
            for (var crateIndex = CrateOffset; crateIndex < CrateOffset + CrateCount; crateIndex++)
            {
                var crate = new Crate(this, Version);
                crate.Read(br);
                Crates.Add(crate);
            }
        }

        public void Write(BinaryWriterExt bw)
        {
            Position.Write(bw);

            bw.WriteU16(CrateOffset);
            bw.WriteU16((ushort)Crates.Count);
            bw.WriteU16(Tilt);
            foreach (var crate in Crates)
            {
                crate.Write(bw);
            }
        }
        public IEnumerator<Crate> GetEnumerator()
        {
            return ((IEnumerable<Crate>)Crates).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Crate>)Crates).GetEnumerator();
        }
    }
}
