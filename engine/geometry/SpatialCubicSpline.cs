using System.Collections.Generic;
using System.Numerics;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public class SpatialCubicSpline : Curve
	{
		CubicSpline1D X { get; set; }
		CubicSpline1D Y { get; set; }
		CubicSpline1D Z { get; set; }
		
		public SpatialCubicSpline(CubicSpline1D x, CubicSpline1D y, CubicSpline1D z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
		
		public SpatialCubicSpline(SortedList<float, float> x, SortedList<float, float> y, SortedList<float, float> z)
		{
			this.X = new CubicSpline1D(x);
			this.Y = new CubicSpline1D(y);
			this.Z = new CubicSpline1D(z);
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
		
		public override Vector3 GetStartPosition()
		{
			return new Vector3(X.GetValueAt(0.0f), Y.GetValueAt(0.0f), Z.GetValueAt(0.0f));
		}
		
		public override Vector3 GetEndPosition()
		{
			return new Vector3(X.GetValueAt(1.0f), Y.GetValueAt(1.0f), Z.GetValueAt(1.0f));
		}
	}
}
