using System.Collections.Generic;
using System.Numerics;
using System;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public class Line : Curve
	{
		Vector3 start;
		Vector3 end;
		Vector3 normal;
		
		public Line(Vector3 start, Vector3 end)
		{
			this.start = start;
			this.end = end;
			
			// No normal supplied, pick arbitrary normal vector:
			Vector3 up = new Vector3(0.0f, 0.0f, 1.0f);
			if (Vector3.Dot(end - start, new Vector3(0.0f, 0.0f, 1.0f)) < 0.1)
			{
				up = new Vector3(0.0f, 1.0f, 0.0f);
			}
			
			this.normal = Vector3.Normalize(Vector3.Cross(end - start, up));
		}
		
		public Line(Vector3 start, Vector3 end, Vector3 normal)
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
