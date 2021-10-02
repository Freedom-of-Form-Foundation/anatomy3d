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
using System.Linq;
using System;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	///     Class <c>LinearSpline1D</c> describes a one-dimensional linear spline, which is a piecewise function.
	///		Each piece is defined by a linear function, \f$q(x) = a_0 + a_1 x, for which the
	///		parameters are defined such that the piecewise function is continuous. A spline is defined by a series of
	///		points that the function must intersect, and the program will automatically generate a curve that passes
	///		through these points. A linear spline is discontinuous on its first derivative.
	/// </summary>
	public class LinearSpline1D : RaytraceableFunction1D
	{
		private float[] _parameters;
		public SortedPointsList<float> Points { get; }
		
		/// <summary>
		///     Construct a linear spline using a set of input points.
		/// 	<example>For example:
		/// 	<code>
		/// 		SortedList<float, float> splinePoints = new SortedList<float, float>();
		/// 		splinePoints.Add(0.0f, 1.1f);
		/// 		splinePoints.Add(0.3f, 0.4f);
		/// 		splinePoints.Add(1.0f, 2.0f);
		/// 		LinearSpline1D spline = new LinearSpline1D(splinePoints);
		/// 	</code>
		/// 	creates a linear spline that passes through three points: (0.0, 1.1), (0.3, 0.4) and (1.0, 2.0).
		/// 	</example>
		/// </summary>
		/// <param name="points">A list of points that is sorted by the x-coordinate. This collection is copied.</param>
		/// <exception cref="ArgumentException">
		/// 	A linear spline must have at least two points to be properly defined. If <c>points</c> contains less than
		/// 	two points, the spline is undefined, so an <c>ArgumentException</c> is thrown.
		/// </exception>
		public LinearSpline1D(SortedList<float, float> points)
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
			
			Points = new SortedPointsList<float>(points);

			// Calculate the coefficients for each segment of the spline:
			_parameters = new float[points.Count];

			// Recursively find the _parameters:
			for (int i = 0; i < points.Count; i++)
			{
				float dx = Points.Key[i] - Points.Key[i - 1];
				float dy = Points.Value[i] - Points.Value[i - 1];

				_parameters[i] = dy / dx;
			}
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
		public float GetNthDerivativeAt(float x, uint derivative)
		{
			// The input parameter must lie between the outer points, and must not be NaN:
			if (!( x >= Points.Key[0] && x <= Points.Key[Points.Count - 1]))
			{
				throw new ArgumentOutOfRangeException("x","Cannot interpolate outside the interval given by the spline points.");
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

			float x1 = Points.Key[i-1];
			float x2 = Points.Key[i];
			float y1 = Points.Value[i-1];
			float y2 = Points.Value[i];
			
			// Calculate and return the interpolated value:
			float dx = x2 - x1;
			float dy = y2 - y1;

			float slope = dy / dx;

			float local_x = x - x1;

			float local_y = local_x * slope;
			float global_y = local_y + y1;

			// Return the result of evaluating a derivative function, depending on the derivative level:
			switch (derivative)
			{
				case 0: return global_y;
				case 1: return slope;
				default: return 0.0f;
			}
		}

		/// <summary>
		///     Get the value of this function \f$q(x)\f$ at the given x-position.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// 	The value that is sampled must lie between the outermost points on which the spline is defined. If
		/// 	<c>x</c> is outside that domain, an <c>ArgumentOutOfRangeException</c> is thrown.
		/// </exception>
		public override float GetValueAt(float x)
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
		public float GetDerivativeAt(float x)
		{
			return GetNthDerivativeAt(x, 1);
		}

		/// <summary>
		///     Solves the equation \f$(q(x))^2 = b_0 + b_1 x + b_2 x^2 + b_3 x^3 + b_4 x^4\f$, returning all values of
		///		\f$x\f$ for which the equation is true. \f$q(x)\f$ is the linear spline. The _parameters z0 and c
		///		can be used to substitute x, such that \f$x = z0 + c t\f$. This is useful for raytracing.
		/// </summary>
		public override IEnumerable<float> SolveRaytrace(QuarticFunction surfaceFunction, float z0 = 0.0f, float c = 1.0f)
		{
			throw new NotImplementedException();
			return null;
		}
	}
}
