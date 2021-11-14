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
using System.Linq;
using System;
using System.Diagnostics;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	///     Class <c>CubicSpline1D</c> describes a one-dimensional cubic spline, which is a piecewise function.
	///		Each piece is defined by a cubic function, \f$q(x) = a_0 + a_1 x + a_2 x^2 + a_3 x^3\f$, for which the
	///		parameters are defined such that the piecewise function is continuous. A spline is defined by a series of
	///		points that the function must intersect, and the program will automatically generate a curve that passes
	///		through these points. A cubic spline is continuous in its derivative and its second derivative. It is
	/// 	therefore 'smoother' than a quadratic spline, but as a result it is not analytically raytracable when used
	/// 	as a heightmap.
	/// </summary>
	public class CubicSpline1D : ContinuousMap<double, double>
	{
		private double[] _parameters;
		public SortedPointsList<double> Points { get; }
		
		/// <summary>
		///     Construct a cubic spline using a set of input points.
		/// 	<example>For example:
		/// 	<code>
		/// 		SortedList<double, double> splinePoints = new SortedList<double, double>();
		/// 		splinePoints.Add(0.0f, 1.1f);
		/// 		splinePoints.Add(0.3f, 0.4f);
		/// 		splinePoints.Add(1.0f, 2.0f);
		/// 		CubicSpline1D spline = new CubicSpline1D(splinePoints);
		/// 	</code>
		/// 	creates a cubic spline that passes through three points: (0.0, 1.1), (0.3, 0.4) and (1.0, 2.0).
		/// 	</example>
		/// </summary>
		/// <param name="points">A list of points that is sorted by the x-coordinate. This collection is copied.</param>
		/// <exception cref="ArgumentException">
		/// 	A cubic spline must have at least two points to be properly defined. If <c>points</c> contains less than
		/// 	two points, the spline is undefined, so an <c>ArgumentException</c> is thrown.
		/// </exception>
		public CubicSpline1D(SortedList<double, double> points)
		{
			if (points.Count < 2)
			{
				if (points.Count == 1)
				{
					throw new ArgumentException("List contains only a single point. A spline must have at least two points.", "points");
				}
				else
				{
					throw new ArgumentException("List is empty. A spline must have at least two points.", "points");
				}
			}
			
			Points = new SortedPointsList<double>(points);
			DebugUtil.AssertAllFinite(Points, nameof(Points));
			
			// Calculate the coefficients of the spline:
			double[] a = new double[points.Count];
			double[] b = new double[points.Count];
			double[] c = new double[points.Count];
			double[] d = new double[points.Count];
			
			// Set up the boundary condition for a natural spline:
			{
				double x2 = 1.0/(Points.Key[1] - Points.Key[0]);
				double y2 = Points.Value[1] - Points.Value[0];

				a[0] = 0.0;
				b[0] = 2.0*x2;
				c[0] = x2;
				d[0] = 3.0*(y2*x2*x2);
			}
			
			// Set up the tridiagonal matrix linear system:
			for (int i = 1; i < points.Count-1; i++) 
			{
				double x1 = 1.0/(Points.Key[i] - Points.Key[i-1]);
				double x2 = 1.0/(Points.Key[i+1] - Points.Key[i]);
				
				double y1 = Points.Value[i] - Points.Value[i-1];
				double y2 = Points.Value[i+1] - Points.Value[i];
				
				a[i] = x1;
				b[i] = 2.0*(x1 + x2);
				c[i] = x2;
				d[i] = 3.0*(y1*x1*x1 + y2*x2*x2);
			}
			
			// Set up the boundary condition for a natural spline:
			{
				double x1 = 1.0/(Points.Key[points.Count-1] - Points.Key[points.Count-2]);
				double y1 = (Points.Value[points.Count-1] - Points.Value[points.Count-2]);
				
				a[points.Count-1] = x1;
				b[points.Count-1] = 2.0*x1;
				c[points.Count-1] = 0.0;
				d[points.Count-1] = 3.0*(y1*x1*x1);
			}
			
			// Solve the linear system using the Thomas algorithm:
			this._parameters = ThomasAlgorithm(a, b, c, d);
		}

		/// <summary>
		///     Get the value of this function \f$q(x)\f$ at the given x-position, or the value of the
		/// 	<c>derivative</c>th derivative of this function. Mathematically, this gives \f$q^{(n)}(x)\f$, where
		/// 	\f$n\f$ is equal to the <c>derivative</c> parameter.
		/// </summary>
		/// <param name="x">The x-coordinate at which the function is sampled.</param>
		/// <param name="derivative">
		/// 	The derivative level that must be taken of the function. If <c>derivative</c> is <c>0</c>, this means
		/// 	no derivative is taken. If it has a value of <c>1</c>, the first derivative is taken, with a value of
		/// 	<c>2</c> the second derivative is taken and so forth. This allows you to take any derivative level
		/// 	of the function.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// 	The value that is sampled must lie between the outermost points on which the spline is defined. If 
		/// 	<c>x</c> is outside that domain, an <c>ArgumentOutOfRangeException</c> is thrown.
		/// </exception>
		public double GetNthDerivativeAt(double x, uint derivative)
		{
			DebugUtil.AssertFinite(x, nameof(x));
			// The input parameter must lie between the outer points, and must not be NaN:
			if (!( x >= Points.Key[0] && x <= Points.Key[Points.Count - 1]))
			{
				throw new ArgumentOutOfRangeException(nameof(x), "Cannot interpolate outside the interval given by the spline points.");
			}
			
			// Find the index `i` of the closest point to the right of the input `x` parameter, which is the right point
			// used to interpolate between. Therefore, `i-1` indicates the left point of the interval.
			int i = Points.Key.BinarySearch(x);
			
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

			double x1 = Points.Key[i-1];
			double x2 = Points.Key[i];
			double y1 = Points.Value[i-1];
			double y2 = Points.Value[i];
			
			// Calculate and return the interpolated value:
			double dx = x2 - x1;
			double dy = y2 - y1;
			
			double a = _parameters[i-1]*dx - dy;
			double b = -_parameters[i]*dx + dy;
			double t = (x-x1)/dx;
			
			// Return a different function depending on the derivative level:
			switch (derivative)
			{
				case 0: return (1.0 - t)*y1 + t*y2 + t*(1.0-t)*((1.0-t)*a + t*b);
				case 1: return (y2 - y1 + 3.0*(a-b)*t*t + (2.0*b - 4.0*a)*t + a)/dx;
				case 2: return (a*(6.0*t - 4.0) + b*(1.0-3.0*t))/(dx*dx);
				case 3: return 6.0*(a-b)/(dx*dx*dx);
				default: return 0.0;
			}
		}

		/// <summary>
		///     Get the value of this function \f$q(x)\f$ at the given x-position.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// 	The value that is sampled must lie between the outermost points on which the spline is defined. If
		/// 	<c>x</c> is outside that domain, an <c>ArgumentOutOfRangeException</c> is thrown.
		/// </exception>
		public override double GetValueAt(double x)
		{
			return GetNthDerivativeAt(x, 0);
		}

		/// <summary>
		///     Get the value of the first derivative of this function \f$q'(x)\f$ at the given x-position.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// 	The value that is sampled must lie between the outermost points on which the spline is defined. If
		/// 	<c>x</c> is outside that domain, an <c>ArgumentOutOfRangeException</c> is thrown.
		/// </exception>
		public double GetDerivativeAt(double x)
		{
			return GetNthDerivativeAt(x, 1);
		}
		
		/// <summary>
		///     Find the solution to a tridiagonal matrix linear system Ax = d using the Thomas algorithm.
		/// </summary>
		private static double[] ThomasAlgorithm(double[] a, double[] b, double[] c, double[] d)
		{
			DebugUtil.AssertAllFinite(a, nameof(a));
			DebugUtil.AssertAllFinite(b, nameof(b));
			DebugUtil.AssertAllFinite(c, nameof(c));
			DebugUtil.AssertAllFinite(d, nameof(d));

			int size = d.Count();
			
			// Perform forward sweep:
			double[] newC = new double[size];
			double[] newD = new double[size];
			
			newC[0] = c[0] / b[0];
			newD[0] = d[0] / b[0];
			for (int i = 1; i < size; i++) 
			{
				newC[i] = c[i] / ( b[i] - a[i]*newC[i-1] );
				newD[i] = (d[i] - a[i]*newD[i-1]) / ( b[i] - a[i]*newC[i-1] );
			}
			DebugUtil.AssertAllFinite(newC, nameof(newC));
			DebugUtil.AssertAllFinite(newD, nameof(newD));
			
			// Perform back substitution:
			double[] x = new double[size];
			
			x[size-1] = newD[size-1];
			for (int i = (size - 2); i >= 0; i--) 
			{
				x[i] = newD[i] - newC[i]*x[i+1];
			}

			DebugUtil.AssertAllFinite(x, nameof(x));
			return x;
		}
	}
}
