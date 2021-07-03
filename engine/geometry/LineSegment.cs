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

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public class LineSegment : Curve
	{
		private Vector3 start;
		private Vector3 end;
		private Vector3 normal;
		
		public LineSegment(Vector3 start, Vector3 end)
		{
			this.start = start;
			this.end = end;
			
			// No normal supplied, pick arbitrary normal vector:
			this.normal = InventNormal(end - start);
		}
		
		public LineSegment(Vector3 start, Vector3 end, Vector3 normal)
		{
			this.start = start;
			this.end = end;
			
			// Ensure that the normal is truly perpendicular to the tangent vector:
			Vector3 tangent = Vector3.Normalize(end - start);
			Vector3 up = Vector3.Normalize(Vector3.Cross(tangent, normal));
			this.normal = Vector3.Normalize(Vector3.Cross(up, tangent));
		}
		
		public override Vector3 GetPositionAt(float t)
		{
			// Simply return the linearly interpolated position between `start` and `end`:
			return t*end + (1.0f-t)*start;
		}
		
		public override Vector3 GetTangentAt(float t)
		{
			// The tangent vector is always in the direction of the line:
			return end - start; // TODO: Should this be normalized, or is length information useful?
		}
		
		public override Vector3 GetNormalAt(float t)
		{
			// The tangent vector is always in the direction of the line:
			return normal; // TODO: Should this be normalized, or is length information useful?
		}
		
		public override Vector3 GetStartPosition()
		{
			return start;
		}
		
		public override Vector3 GetEndPosition()
		{
			return end;
		}
	}
}
