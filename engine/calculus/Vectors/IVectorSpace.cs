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

using System;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus.Vectors
{
    /// <summary>
    /// IVectorSpace types represent the abstract concept of a vector space, distinct from (but possibly convertible
    /// to) other vector spaces. It must be able to evaluate whether it is the same as any other IVectorSpace.
    /// IVectorSpace implementations of Equals must consider IVectorSpace objects of different underlying types
    /// to be unequal.
    ///
    /// IVectorSpace is used to mark mathematical objects intended to map to a specific space as associated with
    /// that space. IVectorSpace objects are propagated through mathematical operations by copying references,
    /// but value equality must still be implemented unless the underlying implementation of the type is a singleton.
    ///
    /// Different objects of the same IVectorSpace type *may* be different vector spaces. Different objects of
    /// different underlying IVectorSpace types are *definitely* different vector spaces. Type identity allows
    /// the compiler to spot incompatible operations; value equality allows these to be detected at runtime if the
    /// type check passes.
    /// </summary>
    public interface IVectorSpace: IEquatable<IVectorSpace>
    {
        /// <summary>
        /// Dimension returns the number of dimensions in this vector space. It must be positive and constant
        /// throughout the life of the vector space.
        /// </summary>
        /// <returns>The dimensionality of this vector space. Must be constant for the life of an object.</returns>
        int Dimension();
    }
}
