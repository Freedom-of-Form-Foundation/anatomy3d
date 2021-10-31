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

using GlmSharp;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	/// <summary>
	///		A <c>LineSegment</c> defines a straight line in 3D space, with a start and end point.
	/// </summary>
	public class LineSegment : Curve
	{
		private dvec3 _start;
		private dvec3 _end;
		private dvec3 _normal;
		
		public LineSegment(dvec3 start, dvec3 end)
		{
			_start = start;
			_end = end;

			// No normal supplied, pick arbitrary normal vector:
			_normal = VectorUtilities.InventNormal(end - start);
		}
		
		public LineSegment(dvec3 start, dvec3 end, dvec3 normal)
		{
			_start = start;
			_end = end;
			
			// Ensure that the normal is truly perpendicular to the tangent vector:
			dvec3 tangent = (end - start).Normalized;
			dvec3 up = dvec3.Cross(tangent, normal).Normalized;
			_normal = dvec3.Cross(up, tangent).Normalized;
		}
		
		/// <inheritdoc />
		public override dvec3 GetPositionAt(double t)
		{
			// Simply return the linearly interpolated position between `start` and `end`:
			return t*_end + (1.0-t)*_start;
		}
		
		/// <inheritdoc />
		public override dvec3 GetTangentAt(double t)
		{
			// The tangent vector is always in the direction of the line:
			return _end - _start; // TODO: Should this be normalized, or is length information useful?
		}
		
		/// <inheritdoc />
		public override dvec3 GetNormalAt(double t)
		{
			// The tangent vector is always in the direction of the line:
			return _normal; // TODO: Should this be normalized, or is length information useful?
		}
		
		/// <inheritdoc />
		public override dvec3 GetStartPosition()
		{
			return _start;
		}
		
		/// <inheritdoc />
		public override dvec3 GetEndPosition()
		{
			return _end;
		}
	}
}
