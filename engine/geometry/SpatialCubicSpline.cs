using System.Collections.Generic;
using System.Numerics;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public class SpatialCubicSpline : Curve
	{
		public CubicSpline1D X { get; }
		public CubicSpline1D Y { get; }
		public CubicSpline1D Z { get; }
		
		public CubicSpline1D NormalX { get; private set; }
		public CubicSpline1D NormalY { get; private set; }
		public CubicSpline1D NormalZ { get; private set; }
		
#region ArbitraryConstants
		// Arbitrary number of interpolating steps for calculating the normal.
		// Four should be enough to prevent sudden jumps.
		private static readonly int kInterpolationStepCount = 4;
#endregion
		
		public SpatialCubicSpline(CubicSpline1D x, CubicSpline1D y, CubicSpline1D z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			
			GenerateCurveNormal();
		}
		
		public SpatialCubicSpline(SortedList<float, float> x, SortedList<float, float> y, SortedList<float, float> z)
		{
			this.X = new CubicSpline1D(x);
			this.Y = new CubicSpline1D(y);
			this.Z = new CubicSpline1D(z);
			
			GenerateCurveNormal();
		}
		
		public SpatialCubicSpline(SortedList<float, Vector3> points)
		{
			// Note: slow implementation, can be optimized in C++.
			SortedList<float, float> x = new SortedList<float, float>();
			SortedList<float, float> y = new SortedList<float, float>();
			SortedList<float, float> z = new SortedList<float, float>();
			
			foreach (var item in points)
			{
				x.Add(item.Key, item.Value.X);
				y.Add(item.Key, item.Value.Y);
				z.Add(item.Key, item.Value.Z);
			}
			
			this.X = new CubicSpline1D(x);
			this.Y = new CubicSpline1D(y);
			this.Z = new CubicSpline1D(z);
			
			GenerateCurveNormal();
		}
		
		public override Vector3 GetPositionAt(float t)
		{
			// Return the components of each 1D spline to get a 3D spline:
			return new Vector3(X.GetValueAt(t), Y.GetValueAt(t), Z.GetValueAt(t));
		}
		
		public override Vector3 GetTangentAt(float t)
		{
			// The tangent vector is always in the direction of the line:
			return new Vector3(X.GetDerivativeAt(t), Y.GetDerivativeAt(t), Z.GetDerivativeAt(t)); // TODO: Should this be normalized, or is length information useful?
		}
		
		public override Vector3 GetNormalAt(float t)
		{
			Vector3 direction = Vector3.Normalize(GetTangentAt(t));
			Vector3 normal = new Vector3(NormalX.GetValueAt(t), NormalY.GetValueAt(t), NormalZ.GetValueAt(t));
			Vector3 up = Vector3.Cross(direction, normal);
			return Vector3.Normalize(Vector3.Cross(up, direction));
		}
		
		public override Vector3 GetStartPosition()
		{
			return new Vector3(X.GetValueAt(0.0f), Y.GetValueAt(0.0f), Z.GetValueAt(0.0f));
		}
		
		public override Vector3 GetEndPosition()
		{
			return new Vector3(X.GetValueAt(1.0f), Y.GetValueAt(1.0f), Z.GetValueAt(1.0f));
		}
		
		private void GenerateCurveNormal()
		{
			// Generate a tangent at key points:
			// TODO: make the precision adapt to the precision of centerCurve!
			
			SortedList<float, float> normalX = new SortedList<float, float>();
			SortedList<float, float> normalY = new SortedList<float, float>();
			SortedList<float, float> normalZ = new SortedList<float, float>();
			
			// No normal supplied, pick arbitrary normal vector:
			Vector3 up = new Vector3(0.0f, 0.0f, 1.0f);
			if (Vector3.Dot(GetTangentAt(0.0f), new Vector3(0.0f, 0.0f, 1.0f)) < 0.1)
			{
				up = new Vector3(0.0f, 1.0f, 0.0f);
			}
			
			// Traverse through all the points in the spline, and calculate the next normal, maintaining smoothness by
			// memorizing the previous normal:
			
			float previousValue = X.PointsX[0]; // X.PointsX is guaranteed to have at least 2 entries, this is safe.
			for (int i = 1; i < X.PointsX.Count; i++)
			{
				float currentValue = X.PointsX[i];
				float segmentSize = currentValue - previousValue;
				
				// Interpolate between each two points:
				for (int j = 0; j < kInterpolationStepCount; j++)
				{
					float t = previousValue + segmentSize*(float)j/(float)kInterpolationStepCount;
					
					Vector3 direction = Vector3.Normalize(GetTangentAt(t));
					Vector3 normal = Vector3.Normalize(Vector3.Cross(direction, up));
					up = Vector3.Cross(normal, direction);
					
					normalX.Add(t, normal.X);
					normalY.Add(t, normal.Y);
					normalZ.Add(t, normal.Z);
				}
				
				previousValue = currentValue;
			}
			
			// Add the final point as well:
			{
				float t = X.PointsX[X.PointsX.Count - 1];
				
				Vector3 direction = Vector3.Normalize(GetTangentAt(t));
				Vector3 normal = Vector3.Normalize(Vector3.Cross(direction, up));
				up = Vector3.Cross(normal, direction);
				
				normalX.Add(t, normal.X);
				normalY.Add(t, normal.Y);
				normalZ.Add(t, normal.Z);
			}
			
			this.NormalX = new CubicSpline1D(normalX);
			this.NormalY = new CubicSpline1D(normalY);
			this.NormalZ = new CubicSpline1D(normalZ);
		}
	}
}
