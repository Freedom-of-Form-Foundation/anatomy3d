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
using GlmSharp;

namespace FreedomOfFormFoundation.AnatomyEngine
{
	/// <summary>
	/// Defines utility methods for vectors.
	/// </summary>
	public static class VectorUtilities
	{
		/// <summary>
		/// Invent a new, arbitrary normal to a vector by using a cross product between one of the axis vectors and
		/// <c>v</c>. The result is not guaranteed to satisfy any properties other than that it is normal to vector
		/// <c>v</c>, and is normalized.
		/// </summary>
		public static dvec3 InventNormal(dvec3 v)
		{
			// If the vector has any infinite or NaN components, throw an error:
			if (Double.IsNaN(dvec3.Dot(dvec3.Ones, v)) || Double.IsInfinity(dvec3.Dot(dvec3.Ones, v)))
			{
				throw new ArgumentException("Vector must not contain infinite or NaN values.", nameof(v));
			}
			
			// TODO: Here it would be nice to add IsNormal instead of an arbitrary comparison, but alas... it's only
			// supported in .NET 5.0 which we can't target yet.
			if ((Math.Abs(v.x) < 0.001) && (Math.Abs(v.y) < 0.001) && (Math.Abs(v.z) < 0.001))
			{
				throw new ArgumentException("Vector is too small.", nameof(v));
			}
			
			dvec3 normalizedV = v.Normalized;
			dvec3 up = dvec3.UnitZ;
			
			// If the vector is pointing in the same direction as the up vector, the cross product can not be trusted.
			// So we use a different up vector.
			if (Math.Sign(normalizedV.z) > 1/Math.Sqrt(2))
			{
				up = dvec3.UnitY;
			}
			
			return dvec3.Cross(normalizedV, up).Normalized;
		}

		/// <summary>
		/// Invent a new, arbitrary normal to a vector by using a cross product between one of the axis vectors and
		/// <c>v</c>. The result is not guaranteed to satisfy any properties other than that it is normal to vector
		/// <c>v</c>, and is normalized.
		/// </summary>
		public static vec3 InventNormal(vec3 v)
		{
			// If the vector has any infinite or NaN components, throw an error:
			if (Single.IsNaN(vec3.Dot(vec3.Ones, v)) || Single.IsInfinity(vec3.Dot(vec3.Ones, v)))
			{
				throw new ArgumentException("Vector must not contain infinite or NaN values.", nameof(v));
			}

			// TODO: Here it would be nice to add IsNormal instead of an arbitrary comparison, but alas... it's only
			// supported in .NET 5.0 which we can't target yet.
			if ((Math.Abs(v.x) < 0.001) && (Math.Abs(v.y) < 0.001) && (Math.Abs(v.z) < 0.001))
			{
				throw new ArgumentException("Vector is too small.", nameof(v));
			}

			vec3 normalizedV = v.Normalized;
			vec3 up = vec3.UnitZ;

			// If the vector is pointing in the same direction as the up vector, the cross product can not be trusted.
			// So we use a different up vector.
			if (Math.Sign(normalizedV.z) > 1 / Math.Sqrt(2))
			{
				up = vec3.UnitY;
			}

			return vec3.Cross(normalizedV, up).Normalized;
		}
	}
}
