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
	// Default implementation is DOUBLE but it can be overridden by the compiler.
	// Specify exactly one of these flags.
	// Warning: Decimal can't represent infinity or NaN, so using these
	// constants won't compile if Real is Decimal.
#if !REALTYPE_DOUBLE && !REALTYPE_FLOAT && !REALTYPE_DECIMAL
#define REALTYPE_DOUBLE
#endif
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
			_v = (double) v;
		}

#pragma mark Constants
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

#pragma mark Conversions
		// Real permits automatic casts <i>from</i> double, float, and decimal. It will not
		// automatically cast to them, since this would prevent the compiler from spotting math
		// being done with native numeric types that needs to be converted to Real.
		public static implicit operator Real(double d) => new Real(d);
		public static implicit operator Real(float f) => new Real(f);
		public static implicit operator Real(decimal m) => new Real(m);
		public static explicit operator double(Real r) => (double) r._v;
		public static explicit operator float(Real r) => (float) r._v;
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

		public override int GetHashCode()
		{
			int h = _v.GetHashCode();
			return ~(h << 16 | h >> 16);
		}
#pragma mark Special value properties
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

#pragma mark Unary operators
		// Note: NaN is neither true nor false.
		public static bool operator true(Real r) => r._v == 0;
		public static bool operator false(Real r) => r._v != 0;

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

#pragma mark Binary arithmetic operators
		public static Real operator +(Real left, Real right) => new Real(right._v + left._v);
		public static Real operator -(Real left, Real right) => new Real(left._v - right._v);
		public static Real operator *(Real left, Real right) => new Real(left._v * right._v);
		public static Real operator /(Real left, Real right) => new Real(left._v / right._v);
		public static Real operator %(Real left, Real right) => new Real(left._v % right._v);
#pragma mark Binary comparison operators
		public static bool operator <(Real left, Real right) => left._v < right._v;
		public static bool operator >(Real left, Real right) => left._v > right._v;
		public static bool operator ==(Real left, Real right) => left._v == right._v;
		public static bool operator !=(Real left, Real right) => left._v != right._v;
		public static bool operator <=(Real left, Real right) => left._v <= right._v;
		public static bool operator >=(Real left, Real right) => left._v >= right._v;
	}
}
