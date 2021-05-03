using System.Collections;
using System.Collections.Generic;
using LibTWOC.Objects;
using LibTWOC.Utils;
namespace LibTWOC.Collections
{
    public class CrateCollection : Serializable, IEnumerable<CrateGroup>
    {
        public List<CrateGroup> Groups = new List<CrateGroup>();
        public uint Version;

        public override void Read(BinaryReaderExt br)
        {
            Groups.ForEach(c => { c.Crates.Clear(); });
            Groups.Clear();

            Version = br.ReadU32(); // ???
            if (Version < 5)
            {
                var groupCount = br.ReadU16();
                Groups = new List<CrateGroup>();
                for (var groupIndex = 0; groupIndex < groupCount; groupIndex++)
                {
                    var group = new CrateGroup(Version);
                    group.Read(br);
                    Groups.Add(group);
                }
            }
        }

        public override void Write(BinaryWriterExt bw)
        {
            bw.WriteU32(Version);
            bw.WriteU16((ushort)Groups.Count);
            foreach (var group in Groups)
            {
                group.Write(bw);
            }
        }

        public IEnumerator<CrateGroup> GetEnumerator()
        {
            return ((IEnumerable<CrateGroup>)Groups).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<CrateGroup>)Groups).GetEnumerator();
        }
    }
}
