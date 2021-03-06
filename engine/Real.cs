﻿/*
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

// Specify REALTYPE_DOUBLE, REALTYPE_FLOAT, or REALTYPE_DECIMAL in your compiler flags to set
// behavior of Real. Otherwise, defaults are DOUBLE for a production build and FLOAT for a
// debug build, to reveal numerical instability.
//
// It is an error to set these flags in multiples.
#if !REALTYPE_DOUBLE && !REALTYPE_FLOAT && !REALTYPE_DECIMAL
 #if DEBUG
  #define REALTYPE_FLOAT
 #else
  #define REALTYPE_DOUBLE
 #endif
#endif

// Error check for multiple REALTYPE declarations. This will get unwieldy as-written if we get
// more than three possible types.
#if (REALTYPE_DOUBLE && (REALTYPE_FLOAT || REALTYPE_DECIMAL)) || (REALTYPE_FLOAT && REALTYPE_DECIMAL)
#error "multiple REALTYPE flags defined - define at most one"
#endif

using System;
using System.Numerics;

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
#if REALTYPE_DOUBLE
		private readonly double _v;
#elif REALTYPE_FLOAT
		private readonly float _v;
#elif REALTYPE_DECIMAL
		private readonly decimal _v;
#else
		#error No known underlying type for Real - see Real.cs for details
#endif

#region Constructors
		/// <summary>
		/// Construct a Real with the value provided.
		/// </summary>
		/// <param name="v">Value this Real should take.</param>
		public Real(double v)
		{
#if REALTYPE_DOUBLE
			_v = v;
#elif REALTYPE_FLOAT
			_v = (float) v;
#elif REALTYPE_DECIMAL
			_v = (decimal) v;
#else
			#error Real doesn't know how to cast from double
#endif
		}

		/// <summary>
		/// Construct a Real with the value provided.
		/// </summary>
		/// <param name="v">Value this Real should take.</param>
		public Real(float v)
		{
#if REALTYPE_DOUBLE
			_v = (double) v;
#elif REALTYPE_FLOAT
			_v = v;
#elif REALTYPE_DECIMAL
			_v = (decimal) v;
#else
			#error Real doesn't know how to cast from float
#endif
		}

		/// <summary>
		/// Construct a Real with the value provided.
		/// </summary>
		/// <param name="v">Value this Real should take.</param>
		public Real(decimal v)
		{
#if REALTYPE_DOUBLE
			_v = (double) v;
#elif REALTYPE_FLOAT
			_v = (float) v;
#elif REALTYPE_DECIMAL
			_v = v;
#else
			#error Real doesn't know how to cast from decimal
#endif
		}

		/// <summary>
		/// Construct a Real with the sbyte value provided.
		/// </summary>
		/// <param name="v">Value this Real should take.</param>
		public Real(sbyte v)
		{
#if REALTYPE_DOUBLE
			_v = (double) v;
#elif REALTYPE_FLOAT
			_v = (float) v;
#elif REALTYPE_DECIMAL
			_v = (decimal) v;
#else
			#error Real doesn't know how to cast from sbyte
#endif
		}

		/// <summary>
		/// Construct a Real with the byte value provided.
		/// </summary>
		/// <param name="v">Value this Real should take.</param>
		public Real(byte v)
		{
#if REALTYPE_DOUBLE
            _v = (double) v;
#elif REALTYPE_FLOAT
			_v = (float) v;
#elif REALTYPE_DECIMAL
            _v = (decimal) v;
#else
             #error Real doesn't know how to cast from byte
#endif
		}

		/// <summary>
		/// Construct a Real with the short value provided.
		/// </summary>
		/// <param name="v">Value this Real should take.</param>
		public Real(short v)
		{
#if REALTYPE_DOUBLE
            _v = (double) v;
#elif REALTYPE_FLOAT
			_v = (float) v;
#elif REALTYPE_DECIMAL
            _v = (decimal) v;
#else
             #error Real doesn't know how to cast from short
#endif
		}

		/// <summary>
		/// Construct a Real with the ushort value provided.
		/// </summary>
		/// <param name="v">Value this Real should take.</param>
		public Real(ushort v)
		{
#if REALTYPE_DOUBLE
            _v = (double) v;
#elif REALTYPE_FLOAT
			_v = (float) v;
#elif REALTYPE_DECIMAL
            _v = (decimal) v;
#else
             #error Real doesn't know how to cast from ushort
#endif
		}

		/// <summary>
		/// Construct a Real with the int value provided.
		/// </summary>
		/// <param name="v">Value this Real should take.</param>
		public Real(int v)
		{
#if REALTYPE_DOUBLE
            _v = (double) v;
#elif REALTYPE_FLOAT
			_v = (float) v;
#elif REALTYPE_DECIMAL
            _v = (decimal) v;
#else
             #error Real doesn't know how to cast from int
#endif
		}

		/// <summary>
		/// Construct a Real with the uint value provided.
		/// </summary>
		/// <param name="v">Value this Real should take.</param>
		public Real(uint v)
		{
#if REALTYPE_DOUBLE
            _v = (double) v;
#elif REALTYPE_FLOAT
			_v = (float) v;
#elif REALTYPE_DECIMAL
            _v = (decimal) v;
#else
             #error Real doesn't know how to cast from uint
#endif
		}

		/// <summary>
		/// Construct a Real with the long value provided.
		/// </summary>
		/// <param name="v">Value this Real should take.</param>
		public Real(long v)
		{
#if REALTYPE_DOUBLE
            _v = (double) v;
#elif REALTYPE_FLOAT
			_v = (float) v;
#elif REALTYPE_DECIMAL
            _v = (decimal) v;
#else
             #error Real doesn't know how to cast from long
#endif
		}

		/// <summary>
		/// Construct a Real with the ulong value provided.
		/// </summary>
		/// <param name="v">Value this Real should take.</param>
		public Real(ulong v)
		{
#if REALTYPE_DOUBLE
            _v = (double) v;
#elif REALTYPE_FLOAT
			_v = (float) v;
#elif REALTYPE_DECIMAL
            _v = (decimal) v;
#else
             #error Real doesn't know how to cast from ulong
#endif
		}

		/// <summary>
		/// Construct a Real that is a copy of another Real.
		/// </summary>
		/// <param name="r">Value to copy.</param>
		public Real(Real r)
		{
			_v = r._v;
		}

#endregion

#region Constants
#if REALTYPE_DOUBLE
		public static readonly Real NaN = new Real(double.NaN);
		public static readonly Real NegativeInfinity = new Real(double.NegativeInfinity);
		public static readonly Real PositiveInfinity = new Real(double.PositiveInfinity);
#elif REALTYPE_FLOAT
		public static readonly Real NaN = new Real(float.NaN);
		public static readonly Real NegativeInfinity = new Real(float.NegativeInfinity);
		public static readonly Real PositiveInfinity = new Real(float.PositiveInfinity);
#elif REALTYPE_DECIMAL
		#warning No definition of NaN, NegativeInfinity, or PositiveInfinity available from decimal
#else
		#error No known source of constants for Real
#endif
#endregion

#region Conversions
		// Real permits automatic casts <i>from</i> all native numeric types. It will not
		// automatically cast to them, since this would prevent the compiler from spotting math
		// being done with native numeric types that needs to be converted to Real.
		public static implicit operator Real(double d) => new Real(d);
		public static implicit operator Real(float f) => new Real(f);
		public static implicit operator Real(decimal m) => new Real(m);
		public static implicit operator Real(sbyte x) => new Real(x);
		public static implicit operator Real(byte x) => new Real(x);
		public static implicit operator Real(short x) => new Real(x);
		public static implicit operator Real(ushort x) => new Real(x);
		public static implicit operator Real(int x) => new Real(x);
		public static implicit operator Real(uint x) => new Real(x);
		public static implicit operator Real(long x) => new Real(x);
		public static implicit operator Real(ulong x) => new Real(x);

		public static explicit operator double(Real real) => (double) real._v;
		public static explicit operator float(Real real) => (float) real._v;
		public static explicit operator decimal(Real real) => (decimal) real._v;
		public static explicit operator sbyte(Real real) => (sbyte) real._v;
		public static explicit operator byte(Real real) => (byte) real._v;
		public static explicit operator short(Real real) => (short) real._v;
		public static explicit operator ushort(Real real) => (ushort) real._v;
		public static explicit operator int(Real real) => (int) real._v;
		public static explicit operator uint(Real real) => (uint) real._v;
		public static explicit operator long(Real real) => (long) real._v;
		public static explicit operator ulong(Real real) => (ulong) real._v;

		/// <summary>
		/// Convert a Real to a Complex with zero imaginary part.
		/// </summary>
		/// <param name="real">Real to convert.</param>
		/// <returns>Complex with real part equal to the provided Real (as double) and zero imaginary part.</returns>
		public static explicit operator Complex(Real real) => new Complex((double) real._v, 0.0);

	#endregion

	#region Interfaces
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return _v.ToString(format, formatProvider);
		}

		public int CompareTo(Real o) => _v.CompareTo(o._v);

		public bool Equals(Real o)
		{
			return _v.Equals(o._v);
		}

		public override bool Equals(object o)
		{
			switch (o)
			{
				case null:
					return false;
				case Real real:
					return this.Equals(real);
				default:
					return false;
			}
		}

		/// <summary>
		/// Calculate a hash code for this value.
		///
		/// Rearranges and inverts the bits of the hash of the underlying type to make it hash
		/// differently.
		/// </summary>
		/// <returns>An arbitrary 32-bit integer, equal between equal instances of this type
		/// with no particular guarantees otherwise, but with an attempt to be scattered. Allows
		/// this type to be used as a key in a hashed data structure (although, frankly, it would
		/// be a pretty weird key type).</returns>
		public override int GetHashCode()
		{
			int h = _v.GetHashCode();
			return ~(h << 16 | h >> 16);
		}
#endregion

#region Special value properties
		public bool IsNaN
		{
			get
			{
#if REALTYPE_DOUBLE
				return double.IsNaN(_v);
#elif REALTYPE_FLOAT
				return float.IsNaN(_v);
#elif REALTYPE_DECIMAL
				return false;
#else
				#error Real doesn't know how to calculate IsNaN for its current type.
#endif
			}
		}

		public bool IsInfinity
		{
			get
			{
#if REALTYPE_DOUBLE
				return double.IsInfinity(_v);
#elif REALTYPE_FLOAT
				return float.IsInfinity(_v);
#elif REALTYPE_DECIMAL
				return false;
#else
				#error Real doesn't know how to calculate IsInfinity for its current type.
#endif
			}
		}

		public bool IsPositiveInfinity
		{
			get
			{
#if REALTYPE_DOUBLE
				return double.IsPositiveInfinity(_v);
#elif REALTYPE_FLOAT
				return float.IsPositiveInfinity(_v);
#elif REALTYPE_DECIMAL
				return false;
#else
#error Real doesn't know how to calculate IsPositiveInfinity for its current type.
#endif
			}
		}

		public bool IsNegativeInfinity
		{
			get
			{
#if REALTYPE_DOUBLE
				return double.IsNegativeInfinity(_v);
#elif REALTYPE_FLOAT
				return float.IsNegativeInfinity(_v);
#elif REALTYPE_DECIMAL
				return false;
#else
#error Real doesn't know how to calculate IsNegativeInfinity for its current type.
#endif
			}
		}

		public bool IsFinite => !IsInfinity && !IsNaN;
#endregion

#region Convenience functions

		/// <summary>
		/// Special cases for nearness checks: handles infinities and NaN. Actually an equality
		/// check in these cases, treating NaN as unequal to NaN.
		/// </summary>
		/// <param name="o">Number to compare this to.</param>
		/// <returns>null if no special case applies; otherwise, result of equality check.</returns>
		private bool? NearSpecialCases(Real o)
		{
			if (IsNaN || o.IsNaN)
			{
				// NaN is not similar to anything else - even itself.
				return false;
			}

			if (IsInfinity)
			{
				// Infinities are only near the exactly identical infinity,
				// regardless of the slop factor.
				return Equals(o);
			}

			if (o.IsInfinity)
			{
				// this isn't infinity or we would have gotten into the previous case.
				return false;
			}

			return null;
		}

		/// <summary>
		/// Compares this Real value for being near another one, plus or minus a certain error range. NaN is not near
		/// anything (including NaN) and infinities are only near the same infinity.
		/// </summary>
		/// <param name="o">Value to compare to.</param>
		/// <param name="slop">Amount of error that can still be considered equal. Nonnegative.</param>
		/// <returns>Whether this is in the closed range of o ± slop.</returns>
		public bool IsAbsolutelyNear(Real o, Real slop)
		{
			if (slop < 0.0)
			{
				throw new ArgumentException("slop cannot be negative.","slop");
			}
			bool? sc = NearSpecialCases(o);
			if (sc != null)
			{
				return sc.Value;
			}

			Real diff = this - o;
			if (diff < 0.0) diff = -diff;
			return diff <= slop;
		}

		/// <summary>
		/// Compares this Real value for being near another one, with a proportional difference no greater than a
		/// certain limit. NaN is not near anything (including NaN), infinities are only near the same infinity,
		/// and zero is only near zero (negative and positive zero are indistinct). Values of different signs are
		/// not relatively near each other. Proportion is relative to the smaller value.
		/// </summary>
		/// <param name="o">Value to compare this to.</param>
		/// <param name="slop">Proportional difference tolerated between values. Nonnegative.</param>
		/// <returns>Whether this is near o, within the given proportion.</returns>
		public bool IsRelativelyNear(Real o, Real slop)
		{
			if (slop < 0.0)
			{
				throw new ArgumentException("slop cannot be negative.", "slop");
			}
			bool? sc = NearSpecialCases(o);
			if (sc != null)
			{
				return sc.Value;
			}

			if (o == 0.0)
			{
				// Zero is not relatively near anything else.
				return this == 0.0;
			}

			Real big, little;
			if (this < 0.0)
			{
				if (o > 0.0) return false;
				// Negative values; "big" and "little" refer to magnitude and the signs will cancel
				// out during division, so reverse the obvious semantics for which goes where.
				if (this > o)
				{
					big = o;
					little = this;
				}
				else
				{
					big = this;
					little = o;
				}
			}
			else
			{
				if (o < 0.0) return false;
				if (this > o)
				{
					big = this;
					little = o;
				}
				else
				{
					big = o;
					little = this;
				}
			}

			Real ratio = big / little;
			return (ratio - 1.0) <= slop;
		}

		/// <summary>
		/// Returns whether this is near the other value, within either an absolute or relative
		/// range. NaN values are never near anything. Infinite values are only near themselves.
		/// Otherwise, values are near each other if the absolute value of the difference between
		/// them is less than the slop value, or if the proportional difference between them is
		/// less than the slop value (calculated relative to the smaller value).
		/// </summary>
		/// <param name="o">Value to compare against.</param>
		/// <param name="slop">Allowable amount of error, either absolute or proportional, for
		/// values to be considered near each other. Nonnegative.</param>
		/// <returns></returns>
		public bool IsNear(Real o, Real slop) => IsAbsolutelyNear(o, slop) || IsRelativelyNear(o, slop);
#endregion

#region Unary operators
		// Note: NaN is neither true nor false.
		public static bool operator true(Real r)
		{
#if (REALTYPE_DOUBLE || REALTYPE_FLOAT)
			if (r.IsNaN) return false;
#endif
			return r._v != 0;
		}

		public static bool operator false(Real r) => r._v == 0.0;

		/// <summary>
		/// Implements logical inversion on a floating-point value, extending the semantics of
		/// C# to do so. NaN values remain NaN. Otherwise, positive and negative zero both
		/// become 1.0, and all nonzero non-NaN values become 0.0.
		/// </summary>
		/// <param name="r">Value to invert.</param>
		/// <returns>Logical inversion of <c>r</c>. NaN remains NaN.</returns>
		public static Real operator !(Real r)
		{
#if !REALTYPE_DECIMAL
			if (r.IsNaN)
			{
				return Real.NaN;
			}
#endif

			if (r._v == 0)
			{
				return new Real(1.0);
			}

			return new Real(0.0);
		}

		public static Real operator +(Real r) => r;
		public static Real operator -(Real r) => new Real(-r._v);
		public static Real operator ++(Real r) => new Real(r._v + 1);
		public static Real operator --(Real r) => new Real(r._v - 1);
#endregion

#region Binary arithmetic operators
		public static Real operator +(Real left, Real right) => new Real(right._v + left._v);
		public static Real operator -(Real left, Real right) => new Real(left._v - right._v);
		public static Real operator *(Real left, Real right) => new Real(left._v * right._v);
		public static Real operator /(Real left, Real right) => new Real(left._v / right._v);
		public static Real operator %(Real left, Real right) => new Real(left._v % right._v);
#endregion

#region Binary comparison operators
		public static bool operator <(Real left, Real right) => left._v < right._v;
		public static bool operator >(Real left, Real right) => left._v > right._v;
		public static bool operator ==(Real left, Real right) => left._v == right._v;
		public static bool operator !=(Real left, Real right) => left._v != right._v;
		public static bool operator <=(Real left, Real right) => left._v <= right._v;
		public static bool operator >=(Real left, Real right) => left._v >= right._v;
#endregion
	}
}
