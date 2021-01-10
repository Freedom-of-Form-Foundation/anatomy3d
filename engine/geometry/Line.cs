using System.Collections.Generic;
using System.Numerics;
using System;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public class Line : Curve
	{
		Vector3 start;
		Vector3 end;
		
		public Line(Vector3 start, Vector3 end)
		{
			this.start = start;
			this.end = end;
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
