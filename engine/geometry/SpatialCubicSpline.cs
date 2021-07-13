﻿/*
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
			this.X = x;
			this.Y = y;
			this.Z = z;
			
			GenerateCurveNormal();
		}
		
		/// <summary>
		/// 	Construct a new 3D cubic spline using three lists of points defining one-dimensional cubic splines.
		/// </summary>
		public SpatialCubicSpline(SortedList<float, float> x, SortedList<float, float> y, SortedList<float, float> z)
			: this(new CubicSpline1D(x), new CubicSpline1D(y), new CubicSpline1D(z)) { }
		
		/// <summary>
		/// 	Construct a new 3D cubic spline using a list of 3D points.
		/// </summary>
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
#endregion Constructors
		
#region Base Class Method Overrides
		/// <inheritdoc />
		public override Vector3 GetPositionAt(float t)
		{
			// Return the components of each 1D spline to get a 3D spline:
			return new Vector3(X.GetValueAt(t), Y.GetValueAt(t), Z.GetValueAt(t));
		}
		
		/// <inheritdoc />
		public override Vector3 GetTangentAt(float t)
		{
			// The tangent vector is always in the direction of the line:
			return new Vector3(X.GetDerivativeAt(t), Y.GetDerivativeAt(t), Z.GetDerivativeAt(t)); // TODO: Should this be normalized, or is length information useful?
		}
		
		/// <inheritdoc />
		public override Vector3 GetNormalAt(float t)
		{
			Vector3 direction = Vector3.Normalize(GetTangentAt(t));
			Vector3 normal = new Vector3(NormalX.GetValueAt(t), NormalY.GetValueAt(t), NormalZ.GetValueAt(t));
			Vector3 up = Vector3.Cross(direction, normal);
			return Vector3.Normalize(Vector3.Cross(up, direction));
		}
		
		/// <inheritdoc />
		public override Vector3 GetStartPosition()
		{
			return new Vector3(X.GetValueAt(0.0f), Y.GetValueAt(0.0f), Z.GetValueAt(0.0f));
		}
		
		/// <inheritdoc />
		public override Vector3 GetEndPosition()
		{
			return new Vector3(X.GetValueAt(1.0f), Y.GetValueAt(1.0f), Z.GetValueAt(1.0f));
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
			
			SortedList<float, float> normalX = new SortedList<float, float>();
			SortedList<float, float> normalY = new SortedList<float, float>();
			SortedList<float, float> normalZ = new SortedList<float, float>();
			
			// No normal supplied, pick arbitrary normal vector and calculate a binormal:
			Vector3 binormal = Vector3.Cross(VectorUtilities.InventNormal(GetTangentAt(0.0f)), GetTangentAt(0.0f));
			
			// Traverse through all the points in the spline, and calculate the next normal, maintaining smoothness by
			// memorizing the previous normal:
			
			float previousValue = X.Points.Key[0]; // X.Points.Key is guaranteed to have at least 2 entries, this is safe.
			for (int i = 1; i < X.Points.Count; i++)
			{
				float currentValue = X.Points.Key[i];
				float segmentSize = currentValue - previousValue;
				
				// Interpolate between each two points:
				for (int j = 0; j < kInterpolationStepCount; j++)
				{
					float t = previousValue + segmentSize*(float)j/(float)kInterpolationStepCount;
					
					Vector3 direction = Vector3.Normalize(GetTangentAt(t));
					Vector3 normal = Vector3.Normalize(Vector3.Cross(direction, binormal));
					binormal = Vector3.Cross(normal, direction);
					
					normalX.Add(t, normal.X);
					normalY.Add(t, normal.Y);
					normalZ.Add(t, normal.Z);
				}
				
				previousValue = currentValue;
			}
			
			// Add the final point as well:
			{
				float t = X.Points.Key[X.Points.Count - 1];
				
				Vector3 direction = Vector3.Normalize(GetTangentAt(t));
				Vector3 normal = Vector3.Normalize(Vector3.Cross(direction, binormal));
				binormal = Vector3.Cross(normal, direction);
				
				normalX.Add(t, normal.X);
				normalY.Add(t, normal.Y);
				normalZ.Add(t, normal.Z);
			}
			
			this.NormalX = new CubicSpline1D(normalX);
			this.NormalY = new CubicSpline1D(normalY);
			this.NormalZ = new CubicSpline1D(normalZ);
		}
#endregion Private Methods
	}
}
