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

using System.Numerics;
using System;

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
		public static Vector3 InventNormal(Vector3 v)
		{
			// If the vector has infinite or NaN components, throw an error:
			if (Single.IsNaN(Vector3.Dot(Vector3.One, v)) || Single.IsInfinity(Vector3.Dot(Vector3.One, v)))
			{
				throw new ArgumentException("Vector must not contain infinite or NaN values.", "v");
			}
			
			// TODO: Here it would be nice to add IsNormal instead of an arbitrary comparison, but alas... it's only
			// supported in .NET 5.0 which we can't target yet.
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
