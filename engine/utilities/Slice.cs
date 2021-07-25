using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FreedomOfFormFoundation.AnatomyEngine
{
    // A Slice is a lightweight, shallow view of part of an underlying IList. It can be used to avoid copying.
    public class Slice<T> : IReadOnlyList<T>
    {
        public Slice(IList<T> list, int offset, int count)
        {
            Original = list;
            Offset = offset;
            Count = count;
            Debug.Assert(count > 0, "Negative-length slice is illegal");
            Debug.Assert(offset > 0, "Negative slice offset is illegal");
            Debug.Assert(offset + count <= list.Count, "Slice too large");
        }

        public int Count { get; }
        public int Offset { get; }
        public IList<T> Original { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return Original.Skip(Offset).Take(Count).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int index]
        {
            get
            {
                Debug.Assert(index < Count, "Slice index out of range");
                return Original[index + Offset];
            }
        }
    }
}
