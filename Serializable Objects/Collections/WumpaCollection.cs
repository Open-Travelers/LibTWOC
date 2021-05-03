using System.Collections.Generic;
using System.Collections;
using LibTWOC.Objects;
using LibTWOC.Utils;
namespace LibTWOC.Collections
{
    public class WumpaCollection : Serializable, IEnumerable<Wumpa>
    {
        public List<Wumpa> Wumpa = new List<Wumpa>();

        public override void Read(BinaryReaderExt br)
        {
            var count = br.ReadU32();
            Wumpa.Clear();
            for (var i = 0; i < count; i++)
            {
                var wumpa = new Wumpa();
                wumpa.Read(br);
                Wumpa.Add(wumpa);
            }
        }

        public override void Write(BinaryWriterExt bw)
        {
            bw.WriteU32((uint)Wumpa.Count);
            for (var i = 0; i < Wumpa.Count; i++)
            {
                var wumpa = Wumpa[i];
                wumpa.Write(bw);
            }
        }
        public IEnumerator<Wumpa> GetEnumerator()
        {
            return ((IEnumerable<Wumpa>)Wumpa).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Wumpa>)Wumpa).GetEnumerator();
        }
    }
}
