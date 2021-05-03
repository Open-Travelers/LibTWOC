using System;
using System.Collections;
using System.Collections.Generic;
using LibTWOC.Objects;
using LibTWOC.Utils;
namespace LibTWOC.Collections
{
    public class AICollection : Serializable, IEnumerable<AI>
    {
        public List<AI> AIs = new List<AI>();
        public AICollection()
        {
            
        }

        public override void Read(BinaryReaderExt br)
        {
            var count = br.ReadU32();
            for (var indexOuter = 0; indexOuter < count; indexOuter++)
            {
                var name = br.ReadString(16);
                var h = br.ReadU32();
                //var type = 0; // FindType(h)
                for (var indexInner = 0; indexInner < h; indexInner++)
                {
                    var ai = new AI(name);
                    ai.Read(br);
                    AIs.Add(ai);
                }
            }
        }

        public override void Write(BinaryWriterExt bw)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<AI> GetEnumerator()
        {
            return ((IEnumerable<AI>)AIs).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<AI>)AIs).GetEnumerator();
        }
    }
}
