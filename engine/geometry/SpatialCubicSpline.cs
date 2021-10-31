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
using GlmSharp;

using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	/// <summary>
	/// 	A <c>SpatialCubicSpline</c> represents a 3D cubic spline curve that smoothly traverses through a series
	/// 	of points. It is defined as the combination of three one-dimensional cubic splines, one for each
	/// 	axis.
	/// </summary>
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
		
#region Constructors
		/// <summary>
		/// 	Construct a new 3D cubic spline using three one-dimensional cubic splines.
		/// </summary>
		public SpatialCubicSpline(CubicSpline1D x, CubicSpline1D y, CubicSpline1D z)
		{
			X = x;
			Y = y;
			Z = z;
			
			GenerateCurveNormal();
		}
		
		/// <summary>
		/// 	Construct a new 3D cubic spline using three lists of points defining one-dimensional cubic splines.
		/// </summary>
		public SpatialCubicSpline(SortedList<double, double> x, SortedList<double, double> y, SortedList<double, double> z)
			: this(new CubicSpline1D(x), new CubicSpline1D(y), new CubicSpline1D(z)) { }
		
		/// <summary>
		/// 	Construct a new 3D cubic spline using a list of 3D points.
		/// </summary>
		public SpatialCubicSpline(SortedList<double, dvec3> points)
		{
			// Note: slow implementation, can be optimized in C++.
			SortedList<double, double> x = new SortedList<double, double>();
			SortedList<double, double> y = new SortedList<double, double>();
			SortedList<double, double> z = new SortedList<double, double>();
			
			foreach (var item in points)
			{
				x.Add(item.Key, item.Value.x);
				y.Add(item.Key, item.Value.y);
				z.Add(item.Key, item.Value.z);
			}
			
			X = new CubicSpline1D(x);
			Y = new CubicSpline1D(y);
			Z = new CubicSpline1D(z);
			
			GenerateCurveNormal();
		}
#endregion Constructors
		
#region Base Class Method Overrides
		/// <inheritdoc />
		public override dvec3 GetPositionAt(double t)
		{
			// Return the components of each 1D spline to get a 3D spline:
			return new dvec3(X.GetValueAt(t), Y.GetValueAt(t), Z.GetValueAt(t));
		}
		
		/// <inheritdoc />
		public override dvec3 GetTangentAt(double t)
		{
			// The tangent vector is always in the direction of the line:
			return new dvec3(X.GetDerivativeAt(t), Y.GetDerivativeAt(t), Z.GetDerivativeAt(t)); // TODO: Should this be normalized, or is length information useful?
		}
		
		/// <inheritdoc />
		public override dvec3 GetNormalAt(double t)
		{
			dvec3 direction = GetTangentAt(t).Normalized;
			dvec3 normal = new dvec3(NormalX.GetValueAt(t), NormalY.GetValueAt(t), NormalZ.GetValueAt(t));
			dvec3 up = dvec3.Cross(direction, normal);
			return dvec3.Cross(up, direction).Normalized;
		}
		
		/// <inheritdoc />
		public override dvec3 GetStartPosition()
		{
			return new dvec3(X.GetValueAt(0.0), Y.GetValueAt(0.0), Z.GetValueAt(0.0));
		}
		
		/// <inheritdoc />
		public override dvec3 GetEndPosition()
		{
			return new dvec3(X.GetValueAt(1.0), Y.GetValueAt(1.0), Z.GetValueAt(1.0));
		}
#endregion Base Class Method Overrides
		
#region Private Methods
		/// <summary>
		/// 	Pre-generate an approximation to the curve's normal by traversing through all the points in the spline.
		/// </summary>
		/// <remarks>
		/// 	This code is called during construction. It must remain ready for an inconsistent state.
		/// </remarks>
		private void GenerateCurveNormal()
		{
			// Generate a tangent at key points:
			// TODO: make the precision adapt to the precision of centerCurve!
			
			SortedList<double, double> normalX = new SortedList<double, double>();
			SortedList<double, double> normalY = new SortedList<double, double>();
			SortedList<double, double> normalZ = new SortedList<double, double>();

			// No normal supplied, pick arbitrary normal vector and calculate a binormal:
			dvec3 binormal = dvec3.Cross(VectorUtilities.InventNormal(GetTangentAt(0.0)), GetTangentAt(0.0));
			
			// Traverse through all the points in the spline, and calculate the next normal, maintaining smoothness by
			// memorizing the previous normal:
			
			double previousValue = X.Points.Key[0]; // Location.Points.Key is guaranteed to have at least 2 entries, this is safe.
			for (int i = 1; i < X.Points.Count; i++)
			{
				double currentValue = X.Points.Key[i];
				double segmentSize = currentValue - previousValue;
				
				// Interpolate between each two points:
				for (int j = 0; j < kInterpolationStepCount; j++)
				{
					double t = previousValue + segmentSize*j/kInterpolationStepCount;

					dvec3 direction = GetTangentAt(t).Normalized;
					dvec3 normal = dvec3.Cross(direction, binormal).Normalized;
					binormal = dvec3.Cross(normal, direction);

					normalX.Add(t, normal.x);
					normalY.Add(t, normal.y);
					normalZ.Add(t, normal.z);
				}
				
				previousValue = currentValue;
			}
			
			// Add the final point as well:
			{
				double t = X.Points.Key[X.Points.Count - 1];
				
				dvec3 direction = GetTangentAt(t).Normalized;
				dvec3 normal = dvec3.Cross(direction, binormal).Normalized;
				binormal = dvec3.Cross(normal, direction);

				normalX.Add(t, normal.x);
				normalY.Add(t, normal.y);
				normalZ.Add(t, normal.z);
			}
			
			NormalX = new CubicSpline1D(normalX);
			NormalY = new CubicSpline1D(normalY);
			NormalZ = new CubicSpline1D(normalZ);
		}
#endregion Private Methods
	}
}
