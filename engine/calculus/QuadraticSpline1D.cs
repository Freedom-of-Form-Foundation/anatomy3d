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
using System;
using GlmSharp;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	/// 	Class <c>QuadraticSpline1D</c> describes a one-dimensional quadratic spline, which is a piecewise function.
	///		Each piece is defined by a quadratic function, \f$q(x) = a_0 + a_1 x + a_2 x^2\f$, for which the _parameters
	///		are defined such that the piecewise function is continuous. A spline is defined by a series of points that
	///		the function must intersect, and the program will automatically generate a curve that passes through these
	///		points. As opposed to a cubic spline, a quadratic spline is not continuous in its second derivative. It is
	///		therefore 'less smooth', but as a result it is possible to raytrace a quadratic spline analytically.
	/// </summary>
	public class QuadraticSpline1D : RaytraceableFunction1D
	{
		private double[] _parameters;
		public SortedPointsList<double> Points { get; }

		/// <summary>
		/// 	Construct a quadratic spline using a set of input points.
		/// 	<example>For example:
		/// 	<code>
		/// 		SortedList<double, double> splinePoints = new SortedList<double, double>();
		/// 		splinePoints.Add(0.0f, 1.1f);
		/// 		splinePoints.Add(0.3f, 0.4f);
		/// 		splinePoints.Add(1.0f, 2.0f);
		/// 		QuadraticSpline1D spline = new QuadraticSpline1D(splinePoints);
		/// 	</code>
		/// 	creates a quadratic spline that passes through three points: (0.0, 1.1), (0.3, 0.4) and (1.0, 2.0).
		/// 	</example>
		/// </summary>
		/// <param name="points">A list of points that is sorted by the x-coordinate. This collection is copied.</param>
		/// <exception cref="ArgumentException">
		/// 	A spline must have at least two points to be properly defined. If <c>points</c> contains less than two
		/// 	points, the spline is undefined, so an <c>ArgumentException</c> is thrown.
		/// </exception>
		public QuadraticSpline1D(SortedList<double, double> points)
		{
			if (points.Count < 2)
			{
				if (points.Count == 1)
				{
					throw new ArgumentException("List contains only a single point. A spline must have at least two points.", nameof(points));
				}
				else
				{
					throw new ArgumentException("List is empty. A spline must have at least two points.", nameof(points));
				}
			}
			
			Points = new SortedPointsList<double>(points);
			
			// Calculate the coefficients of the spline:
			_parameters = new double[points.Count];
			
			// Find the first segment parameter, which will be a straight line:
			{
				double dx = Points.Key[1] - Points.Key[0];
				double dy = Points.Value[1] - Points.Value[0];
				
				_parameters[0] = dy/dx;
			}
			
			// Recursively find the other _parameters:
			for (int i = 1; i < points.Count; i++) 
			{
				double dx = Points.Key[i] - Points.Key[i-1];
				double dy = Points.Value[i] - Points.Value[i-1];
				
				_parameters[i] = -_parameters[i-1] + 2.0f*dy/dx;
			}
			
		}
		
		/// <summary>
		/// 	Get the value of this function \f$q(x)\f$ at the given x-position, or the value of the
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
		public override double GetNthDerivativeAt(double x, uint derivative)
		{
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

			double a = -_parameters[i]*dx + dy;
			double t = (x-x1)/dx;

			// Return a different function depending on the derivative level:
			switch (derivative)
			{
				case 0: return (1.0 - t)*y1 + t*y2 + t*(1.0-t)*a;
				case 1: return (dy + a * (1.0 - 2.0 * t))/dx;
				case 2: return (2.0 * a * t)/(dx*dx);
				default: return 0.0;
			}
		}
		
		/// <summary>
		/// 	Get the value of this function \f$q(x)\f$ at the given x-position.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// 	The value that is sampled must lie between or on the outermost points on which the spline is defined. If 
		/// 	<c>x</c> is outside that domain, an <c>ArgumentOutOfRangeException</c> is thrown.
		/// </exception>
		public override double GetValueAt(double x)
		{
			return GetNthDerivativeAt(x, 0);
		}
		
		/// <summary>
		/// 	Solves the equation \f$(q(x))^2 = b_0 + b_1 x + b_2 x^2 + b_3 x^3 + b_4 x^4\f$, returning all values of
		///		\f$x\f$ for which the equation is true. \f$q(x)\f$ is the quadratic spline. The _parameters z0 and c
		///		can be used to substitute x, such that \f$x = z0 + c t\f$. This is useful for raytracing.
		/// </summary>
		public override IEnumerable<double> SolveRaytrace(QuarticFunction surfaceFunction, double z0 = 0.0, double c = 1.0)
		{
			// Solve the polynomial equation for each segment:
			for (int i = 1; i < Points.Count; i++)
			{
				double x1 = Points.Key[i-1];
				double x2 = Points.Key[i];
				double y1 = Points.Value[i-1];
				double y2 = Points.Value[i];

				// Calculate and return the interpolated value:
				double dx = x2 - x1;
				double div = 1.0/dx;
				double dy = y2 - y1;

				double a = -_parameters[i]*dx + dy;

				// Write in the form of a0 + a1 z + a2 z^2:
				double a0 = -a*x1*x1*div*div - (a + dy)*x1*div + y1;
				double a1 = 2.0*a*x1*div*div + (a + dy)*div;
				double a2 = -a*div*div;

				// Substitute z = z0 + c t:
				double A0 = a0 + a1*z0 + a2*z0*z0;
				double A1 = (a1 + 2.0*a2*z0)*c;
				double A2 = a2*c*c;

				// Find the quartic polynomial to solve:
				double p0 = surfaceFunction.a0 - A0*A0;
				double p1 = surfaceFunction.a1 - 2.0*A0*A1;
				double p2 = surfaceFunction.a2 - (2.0*A0*A2 + A1*A1);
				double p3 = surfaceFunction.a3 - 2.0*A1*A2;
				double p4 = surfaceFunction.a4 - A2*A2;

				// Solve the quartic polynomial:
				IEnumerable<double> intersections = QuarticFunction.Solve(p0, p1, p2, p3, p4);
				
				// Only return the value if it is sampled within the segment that we are currently considering,
				// otherwise the value we got is invalid:
				foreach (var j in intersections)
				{
					if (((z0 + c*j) > x1) && ((z0 + c*j) <= x2))
					{
						yield return j;
					}
				}
			}
		}

		/// <summary>
		/// 	Solves the Monge Gauge equation \f$\vec{r_0} + t \vec{r_1} = \langle u, v, f(u,v) \rangle\f$, returning
		///		all values of\f$t\f$ for which the equation is true. \f$f(u, v)\f$ is the quadratic spline.
		/// </summary>
		public override IEnumerable<double> SolveRaytracePlanar(dvec3 r0, dvec3 r1)
		{
			// Solve the polynomial equation for each segment:
			for (int i = 1; i < Points.Count; i++)
			{
				double x1 = Points.Key[i - 1];
				double x2 = Points.Key[i];
				double y1 = Points.Value[i - 1];
				double y2 = Points.Value[i];

				// Calculate and return the interpolated value:
				double dx = x2 - x1;
				double div = 1.0 / dx;
				double dy = y2 - y1;

				double a = -_parameters[i] * dx + dy;

				// Write in the form of a0 + a1 z + a2 z^2:
				double a0 = -a * x1 * x1 * div * div - (a + dy) * x1 * div + y1;
				double a1 = 2.0 * a * x1 * div * div + (a + dy) * div;
				double a2 = -a * div * div;

				// Substitute u = u0 + d0 t:
				double A0 = a0 + a1 * r0.x + a2 * r0.x * r0.x - r0.z;
				double A1 = a1 * r1.x + 2.0*a2*r0.x*r1.x - r1.z;
				double A2 = a2 * r1.x * r1.x;

				// Solve the quartic polynomial:
				IEnumerable<double> intersections = QuadraticFunction.Solve(A0, A1, A2);

				// Only return the value if it is sampled within the segment that we are currently considering,
				// otherwise the value we got is invalid:
				foreach (var j in intersections)
				{
					if (((r0.x + r1.x * j) > x1) && ((r0.x + r1.x * j) <= x2))
					{
						yield return j;
					}
				}
			}
		}
	}
}
