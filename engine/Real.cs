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
// IDE configuration: This file will contain some redundant casts because it's
// designed to allow a field type to be changed to change the behavior of
// the rest of the file. Other IDEs with different "disable for file"
// comment syntax should put those disables here.
// ReSharper disable RedundantCast

namespace FreedomOfFormFoundation.AnatomyEngine
{
    /// <summary>
    /// Struct <c>Real</c> wraps a floating-point representation, which can be changed at compile time to reveal
    /// numerical instability problems (by using float) or to minimize them (by using double). It also offers a window
    /// to add debugging hooks for math during development, watch for numerical instability, etc.
    /// </summary>
    public struct Real : IComparable<Real>, IEquatable<Real>, IFormattable
    {
        /// <summary>
        /// _v contains the value represented by this Real. To change the effective type of Real,
        /// change the type of this and the special constants below the constructors.
        /// </summary>
        private readonly double _v;

        #pragma mark Constructors
        /// <summary>
        /// Construct a Real with the value provided.
        /// </summary>
        /// <param name="v">Value this Real should take.</param>
        public Real(double v)
        {
            _v = v;
        }

        /// <summary>
        /// Construct a Real with the value provided.
        /// </summary>
        /// <param name="v">Value this Real should take.</param>
        public Real(float v)
        {
            _v = v;
        }

        /// <summary>
        /// Construct a Real with the value provided.
        /// </summary>
        /// <param name="v">Value this Real should take.</param>
        public Real(decimal v)
        {
            _v = (double)v;
        }

        #pragma mark Conversions
        // Real permits automatic casts <i>from</i> double, float, and decimal. It will not
        // automatically cast to them, since this would prevent the compiler from spotting math
        // being done with native numeric types that needs to be converted to Real.
        public static implicit operator Real(double d) => new Real(d);
        public static implicit operator Real(float f) => new Real(f);
        public static implicit operator Real(decimal m) => new Real(m);
        public static explicit operator double(Real r) => (double)r._v;
        public static explicit operator float(Real r) => (float)r._v;
        public static explicit operator decimal(Real r) => (decimal) r._v;

        #pragma mark Interfaces
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return _v.ToString(format, formatProvider);
        }

        public int CompareTo(Real o) => _v.CompareTo(o._v);

        public bool Equals(Real o)
        {
            return _v.Equals(o._v);
        }

    }
}
