/*
 * Copyright (C) 2021 Freedom of Form Foundation, Inc.
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License, version 2 (GPLv2) as published by the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License, version 2 (GPLv2) for more details.
 *
 * You should have received a copy of the GNU General Public License, version 2 (GPLv2)
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FreedomOfFormFoundation.AnatomyEngine
{
    /// <summary>
    /// A Slice is a lightweight, shallow view of part of an underlying IList. It can be used to avoid copying.
    /// </summary>
    /// <typeparam name="T">Type stored in the underlying IList.</typeparam>
    public readonly struct Slice<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// Construct a Slice viewing a specific list at a specific range.
        /// </summary>
        /// <remarks>Slice performs bounds-checking in debug builds, but skips it in release builds. It is intended
        /// to be used as a higher-performance alternative to GetRange when the original list will not change and
        /// no copy is explicitly desired.</remarks>
        /// <param name="list">List to slice.</param>
        /// <param name="offset">The index of the original list that should correspond to the 0th index of the slice;
        /// the first index that is in range (if the slice has a length greater than 0).</param>
        /// <param name="count">The number of items to include in the slice. The last included item of the underlying
        /// list will be the item at index <c>offset + count - 1</c>.</param>
        public Slice(IList<T> list, int offset, int count)
        {
            Original = list;
            Offset = offset;
            Count = count;
            Debug.Assert(count > 0, "Negative-length slice is illegal");
            Debug.Assert(offset > 0, "Negative slice offset is illegal");
            Debug.Assert(offset + count <= list.Count, "Slice too large");
        }

        /// <summary>
        /// Number of items in this slice.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Location in the original list where this slice starts.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// The list this slice is viewing into. If the list changes, the slice may become invalid by referring
        /// to indexes that don't exist.
        /// </summary>
        public IList<T> Original { get; }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return Original.Skip(Offset).Take(Count).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Retrieve one element of this slice.
        /// </summary>
        /// <remarks>The index is relative to the base of this slice, not the original, underlying list.</remarks>
        /// <param name="index">Index into the slice to retrieve a value from.</param>
        public T this[int index]
        {
            get
            {
                Debug.Assert(index < Count, "Slice index out of range");
                Debug.Assert(index >= 0, "Slice index negative");
                return Original[index + Offset];
            }
        }
    }
}
