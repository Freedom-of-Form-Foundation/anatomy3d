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
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus.Vectors
{
    // A Vector represents a spatial vector in zero or more dimensions, existing in a specific vector space
    // throughout its life. A Vector is immutable (operations will give new Vectors).
    public struct Vector<TSpace> : IEquatable<Vector<TSpace>> where TSpace : IVectorSpace
    {
        public TSpace Space { get; }
        private readonly Real[] _v;

        /// <summary>
        /// Construct a Vector in the provided space, with the provided values. There must be exactly as many values
        /// as there are dimensions in the vector space.
        /// </summary>
        /// <param name="space">Vector space for this vector.</param>
        /// <param name="values">Values in this vector.</param>
        public Vector(TSpace space, IEnumerable<Real> values)
        {
            Debug.Assert(space != null);
            _v = values.ToArray();
            Space = space;
            Debug.Assert(_v.Length == space.Dimension(),
                "Bad Vector size.",
                String.Format("{0} values provided for {1}-dimensional IVectorSpace.", _v.Length, space.Dimension()));
        }

        #region Convenience constructors
        /// <summary>
        /// <see cref="Vector{TSpace}(TSpace, IEnumerable{Real})"/>.
        /// </summary>
        /// <param name="space">Vector space for this vector.</param>
        /// <param name="values">Values in this vector.</param>
        public Vector(TSpace space, params Real[] values) : this(space, (IEnumerable<Real>)values)
        {
        }

        /// <summary>
        /// Construct a Vector in the provided space, with the provided values (which will be converted to Real). There
        /// must be exactly as many values as there are dimensions in the vector space.
        /// </summary>
        /// <param name="space">Vector space for this vector.</param>
        /// <param name="values">Values in this vector.</param>
        public Vector(TSpace space, IEnumerable<double> values) : this(space, from x in values select new Real(x))
        {
        }

        /// <summary>
        /// <see cref="Vector{TSpace}(TSpace, IEnumerable{double})"/>.
        /// </summary>
        /// <param name="space">Vector space for this vector.</param>
        /// <param name="values">Values in this vector.</param>
        public Vector(TSpace space, params double[] values) : this(space, (IEnumerable<double>) values)
        {
        }

        /// <summary>
        /// <see cref="Vector{TSpace}(TSpace, IEnumerable{double})"/>.
        /// </summary>
        /// <param name="space">Vector space for this vector.</param>
        /// <param name="values">Values in this vector.</param>
        public Vector(TSpace space, IEnumerable<float> values) : this(space, from x in values select new Real(x))
        {
        }

        /// <summary>
        /// <see cref="Vector{TSpace}(TSpace, IEnumerable{double})"/>.
        /// </summary>
        /// <param name="space">Vector space for this vector.</param>
        /// <param name="values">Values in this vector.</param>
        public Vector(TSpace space, params float[] values) : this(space, (IEnumerable<float>) values)
        {
        }
        #endregion

        #region Private constructors
        /// <summary>
        /// Constructs a Vector adopting the provided space and data. Performs no validation. Requires the provided
        /// array of Real to have the correct size and to not change.
        ///
        /// Most mathematical operations in the Vector family of classes will use this constructor.
        ///
        /// The order of arguments is reversed to distinguish this from the public params version, which copies
        /// the provided values rather than adopting the array internally. This extra copy can be skipped when
        /// we control the code generating the data.
        /// </summary>
        /// <param name="space">Vector space to use.</param>
        /// <param name="values">Array of Real values to adopt. Must be the same length as the Dimension() of the
        /// vector space. Must never change after construction (it is incorporated, not copied).</param>
        private Vector(Real[] values, TSpace space)
        {
            Space = space;
            _v = values;
        }
        #endregion

        /// <summary>
        /// The number of dimensions in this vector.
        /// </summary>
        public int Dim => Space.Dimension();

        /// <summary>
        /// Read the <c>i</c>th element of this vector.
        /// </summary>
        /// <param name="i">Index of item to read.</param>
        public Real this[int i] => _v[i];

        public bool Equals(Vector<TSpace> other)
        {
            return _v.Equals(other._v) && EqualityComparer<TSpace>.Default.Equals(Space, other.Space);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector<TSpace> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_v.GetHashCode() * 397) ^ EqualityComparer<TSpace>.Default.GetHashCode(Space);
            }
        }
    }
}
