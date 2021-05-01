using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System;
using RootFinding = MathNet.Numerics.RootFinding;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	public class CubicSpline1D : ContinuousMap<float, float>
	{
		private float[] parameters;
		public List<float> PointsX { get; }
		public List<float> PointsY { get; }
		
		public CubicSpline1D(SortedList<float, float> points)
		{
			if (points.Count < 2)
			{
				if (points.Count == 1)
				{
					throw new ArgumentException("points","List contains only a single point. A spline must have at least two points.");
				}
				else
				{
					throw new ArgumentException("points","List is empty. A spline must have at least two points.");
				}
			}
			
			PointsX = points.Keys.ToList();
			PointsY = points.Values.ToList();
			
			// Calculate the coefficients of the spline:
			float[] a = new float[points.Count];
			float[] b = new float[points.Count];
			float[] c = new float[points.Count];
			float[] d = new float[points.Count];
			
			// Set up the boundary condition for a natural spline:
			{
				float x2 = 1.0f/(PointsX[1] - PointsX[0]);
				float y2 = PointsY[1] - PointsY[0];
				
				a[0] = 0.0f;
				b[0] = 2.0f*x2;
				c[0] = x2;
				d[0] = 3.0f*(y2*x2*x2);
			}
			
			// Set up the tridiagonal matrix linear system:
			for (int i = 1; i < points.Count-1; i++) 
			{
				float x1 = 1.0f/(PointsX[i] - PointsX[i-1]);
				float x2 = 1.0f/(PointsX[i+1] - PointsX[i]);
				
				float y1 = PointsY[i] - PointsY[i-1];
				float y2 = PointsY[i+1] - PointsY[i];
				
				a[i] = x1;
				b[i] = 2.0f*(x1 + x2);
				c[i] = x2;
				d[i] = 3.0f*(y1*x1*x1 + y2*x2*x2);
			}
			
			// Set up the boundary condition for a natural spline:
			{
				float x1 = 1.0f/(PointsX[points.Count-1] - PointsX[points.Count-2]);
				float y1 = (PointsY[points.Count-1] - PointsY[points.Count-2]);
				
				a[points.Count-1] = x1;
				b[points.Count-1] = 2.0f*x1;
				c[points.Count-1] = 0.0f;
				d[points.Count-1] = 3.0f*(y1*x1*x1);
			}
			
			// Solve the linear system using the Thomas algorithm:
			this.parameters = ThomasAlgorithm(a, b, c, d);
		}
		
		public float GetAt(float x, uint derivative)
		{
			// The input parameter must lie between the outer points:
			if ((x < PointsX[0]) || (x > PointsX[PointsX.Count - 1]))
			{
				throw new ArgumentOutOfRangeException("x","Cannot interpolate outside the interval given by the spline points.");
			}
			
			// Find the index `i` of the closest point to the right of the input `x` parameter, which is the right point
			// used to interpolate between. Therefore, `i-1` indicates the left point of the interval.
			int i = PointsX.BinarySearch(x);
			
			// BinarySearch returns a bitwise complement of the index if the point is not exactly in the list, such as
			// when interpolating. To turn it into a valid index, we take the bitwise complement again if it is negative:
			if (i < 0)
			{
				i = ~i;
			}
			
			// If the index is zero, we are exactly on the first point in the list. We increment by one to get the value
			// of the first spline segment, to avoid an IndexOutOfRangeException later on:
			if (i == 0)
			{
				i++;
			}

			float x1 = PointsX[i-1];
			float x2 = PointsX[i];
			float y1 = PointsY[i-1];
			float y2 = PointsY[i];
			
			// Calculate and return the interpolated value:
			float dx = x2 - x1;
			float dy = y2 - y1;
			
			float a = parameters[i-1]*dx - dy;
			float b = -parameters[i]*dx + dy;
			float t = (x-x1)/dx;
			
			// Return a different function depending on the derivative level:
			float returnValue = 0.0f;
			switch (derivative)
			{
				case 0:
					returnValue = (1.0f - t)*y1 + t*y2 + t*(1.0f-t)*((1.0f-t)*a + t*b);
					break;
				case 1:
					returnValue = (y2 - y1 + 3.0f*(a-b)*t*t + (2.0f*b - 4.0f*a)*t + a)/dx;
					break;
				case 2:
					returnValue = (a*(6.0f*t - 4.0f) + b*(1.0f-3.0f*t))/(dx*dx);
					break;
				case 3:
					returnValue = 6.0f*(a-b)/(dx*dx*dx);
					break;
				default:
					returnValue = 0.0f;
					break;
			}
			
			return returnValue;
		}
		
		public override float GetValueAt(float x)
		{
			return GetAt(x, 0);
		}
		
		public float GetDerivativeAt(float x)
		{
			return GetAt(x, 1);
		}
		
		/// <summary>
		///     Find the solution to a tridiagonal matrix linear system Ax = d using the Thomas algorithm.
		/// </summary>
		private static float[] ThomasAlgorithm(float[] a, float[] b, float[] c, float[] d)
		{
			int size = d.Count();
			
			// Perform forward sweep:
			float[] newC = new float[size];
			float[] newD = new float[size];
			
			newC[0] = c[0] / b[0];
			newD[0] = d[0] / b[0];
			for (int i = 1; i < size; i++) 
			{
				newC[i] = c[i] / ( b[i] - a[i]*newC[i-1] );
				newD[i] = (d[i] - a[i]*newD[i-1]) / ( b[i] - a[i]*newC[i-1] );
			}
			
			// Perform back substitution:
			float[] x = new float[size];
			
			x[size-1] = newD[size-1];
			for (int i = (size - 2); i >= 0; i--) 
			{
				x[i] = newD[i] - newC[i]*x[i+1];
			}
			
			return x;
		}
	}
}
