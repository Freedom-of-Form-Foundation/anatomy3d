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

using System.Collections.Generic;
using System.Numerics;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	/// <summary>
	///     A <c>Curve</c> defines any finite one-dimensional path in 3D space, with a start and end point.
	/// </summary>
	public abstract class Curve : ContinuousMap<float, Vector3>
	{
		/// <summary>
		///     Returns the position in 3D space of the curve based on the parametric variable <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The parameter along the length of the curve. <paramref name="t"/> must be in the range
		/// \f$[ 0, 1 ]\f$.</param>
		public abstract Vector3 GetPositionAt(float t);
		
		/// <summary>
		///     Returns the tangent vector of the curve based on the parametric variable <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The parameter along the length of the curve. <paramref name="t"/> must be in the range
		/// \f$[ 0, 1 ]\f$.</param>
		public abstract Vector3 GetTangentAt(float t);
		
		/// <summary>
		///     Returns the normal vector of the curve based on the parametric variable <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The parameter along the length of the curve. <paramref name="t"/> must be in the range
		/// \f$[ 0, 1 ]\f$.</param>
		public abstract Vector3 GetNormalAt(float t);
		
		/// <summary>
		///     Returns the binormal vector of the curve based on the parametric variable <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The parameter along the length of the curve. <paramref name="t"/> must be in the range
		/// \f$[ 0, 1 ]\f$.</param>
		public virtual Vector3 GetBinormalAt(float t)
		{
			return Vector3.Cross(GetTangentAt(t), GetNormalAt(t));
		}
		
		/// <summary>
		///     Returns the position in 3D space of the start point of the curve.
		///     Should return the same value as <c>getPositionAt(0.0)</c>.
		/// </summary>
		public virtual Vector3 GetStartPosition()
		{
			return GetPositionAt(0.0f);
		}
		
		/// <summary>
		///     Returns the position in 3D space of the end point of the curve.
		///     Should return the same value as <c>getPositionAt(1.0)</c>.
		/// </summary>
		public virtual Vector3 GetEndPosition()
		{
			return GetPositionAt(1.0f);
		}
		
		/// <inheritdoc />
		public override Vector3 GetValueAt(float t)
		{
			return GetPositionAt(t);
		}
		
		/// <summary>
		/// Invent a new, arbitrary normal to a vector by using a cross product between one of the axis vectors and
		/// <c>v</c>. The result is not guaranteed to satisfy any properties other than that it is normal to vector
		/// <c>v</c>, and is normalized.
		/// </summary>
		protected static Vector3 InventNormal(Vector3 v)
		{
			// If the vector has infinite or NaN components, throw an error:
			if (Single.IsNaN(Vector3.Dot(Vector3.One, v)) || Single.IsInfinity(Vector3.Dot(Vector3.One, v)))
			{
				throw new ArgumentException("Vector must not contain infinite or NaN values.", "v");
			}
			
			// TODO: Here it would be nice to add IsNormal instead of an arbitrary comparison, but alas...
			if ((Math.Abs(v.X) < 0.001) && (Math.Abs(v.Y) < 0.001) && (Math.Abs(v.Z) < 0.001))
			{
				throw new ArgumentException("Vector is too small.", "v");
			}
			
			Vector3 normalizedV = Vector3.Normalize(v);
			Vector3 up = Vector3.UnitZ;
			
			// If the vector is pointing in the same direction as the up vector, the cross product can not be trusted.
			// So we use a different up vector.
			if (Math.Sign(normalizedV.Z) > 1/Math.Sqrt(2))
			{
				up = Vector3.UnitY;
			}
			
			return Vector3.Normalize(Vector3.Cross(normalizedV, up));
		}
	}
}
