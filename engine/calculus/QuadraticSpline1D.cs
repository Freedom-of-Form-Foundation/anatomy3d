using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	///     Class <c>QuadraticSpline1D</c> describes a one-dimensional quadratic spline, which is a piecewise function.
	///		Each piece is defined by a quadratic function, \f$q(x) = a_0 + a_1 x + a_2 x^2\f$, for which the parameters
	///		are defined such that the piecewise function is continuous. A spline is defined by a series of points that
	///		the function must intersect, and the program will automatically generate a curve that passes through these
	///		points. As opposed to a cubic spline, a quadratic spline is not continuous in its second derivative. It is
	///		therefore 'less smooth', but as a result it is possible to raytrace a quadratic spline analytically.
	/// </summary>
	public class QuadraticSpline1D : ContinuousMap<float, float>
	{
		private float[] parameters;
		public List<float> PointsX { get; }
		public List<float> PointsY { get; }
		
		/// <summary>
		///     Construct a quadratic spline using a set of input points.
		/// 	<example>For example:
		/// 	<code>
		/// 		SortedList<float, float> splinePoints = new SortedList<float, float>();
		/// 		splinePoints.Add(0.0f, 1.1f);
		/// 		splinePoints.Add(0.3f, 0.4f);
		/// 		splinePoints.Add(1.0f, 2.0f);
		/// 		QuadraticSpline1D spline = new QuadraticSpline1D(splinePoints);
		/// 	</code>
		/// 	creates a quadratic spline that passes through three points: (0.0, 1.1), (0.3, 0.4) and (1.0, 2.0).
		/// 	</example>
		/// </summary>
		/// <param name="points">A list of points that is sorted by the x-coordinate.</param>
		/// <exception cref="ArgumentException">
		/// 	A spline must have at least two points to be properly defined. If <c>points</c> contains less than two
		/// 	points, the spline is undefined, so an <c>ArgumentException</c> is thrown.
		/// </exception>
		public QuadraticSpline1D(SortedList<float, float> points)
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
			parameters = new float[points.Count];
			
			// Find the first segment parameter, which will be a straight line:
			{
				float dx = PointsX[1] - PointsX[0];
				float dy = PointsY[1] - PointsY[0];
				
				parameters[0] = dy/dx;
			}
			
			// Recursively find the other parameters:
			for (int i = 1; i < points.Count; i++) 
			{
				float dx = PointsX[i] - PointsX[i-1];
				float dy = PointsY[i] - PointsY[i-1];
				
				parameters[i] = -parameters[i-1] + 2.0f*dy/dx;
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
			
			float a = -parameters[i]*dx + dy;
			float t = (x-x1)/dx;
			
			// Return a different function depending on the derivative level:
			float returnValue = 0.0f;
			switch (derivative)
			{
				case 0:
					returnValue = (1.0f - t)*y1 + t*y2 + t*(1.0f-t)*a;
					break;
				case 1:
					returnValue = (dy + a * (1.0f - 2.0f * t))/dx;
					break;
				case 2:
					returnValue = (2.0f * a * t)/(dx*dx);
					break;
				default:
					returnValue = 0.0f;
					break;
			}
			
			return returnValue;
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
			return GetAt(x, 0);
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
			return GetAt(x, 1);
		}
		
		/// <summary>
		///     Solves the equation \f$(q(x))^2 = b_0 + b_1 x + b_2 x^2 + b_3 x^3 + b_4 x^4\f$, returning all values of
		///		\f$x\f$ for which the equation is true. \f$q(x)\f$ is the quadratic spline. The parameters z0 and c
		///		can be used to substitute x, such that \f$x = z0 + c t\f$. This is useful for raytracing.
		/// </summary>
		public override List<float> SolveRaytrace(QuarticFunction surfaceFunction, float z0 = 0.0f, float c = 1.0f)
		{
			List<float> output = new List<float>();
			
			// Solve the polynomial equation for each segment:
			for (int i = 1; i < PointsX.Count; i++)
			{
				float x1 = PointsX[i-1];
				float x2 = PointsX[i];
				float y1 = PointsY[i-1];
				float y2 = PointsY[i];
				
				// Calculate and return the interpolated value:
				float dx = x2 - x1;
				float div = 1.0f/dx;
				float dy = y2 - y1;
				
				float a = -parameters[i]*dx + dy;
				
				// Write in the form of a0 + a1 z + a2 z^2:
				float a0 = -a*x1*x1*div*div - (a + dy)*x1*div + y1;
				float a1 = 2.0f*a*x1*div*div + (a + dy)*div;
				float a2 = -a*div*div;
				
				// Substitute z = z0 + c t:
				float A0 = a0 + a1*z0 + a2*z0*z0;
				float A1 = (a1 + 2.0f*a2*z0)*c;
				float A2 = a2*c*c;
				
				// Find the quartic polynomial to solve:
				float p0 = surfaceFunction.a0 - A0*A0;
				float p1 = surfaceFunction.a1 - 2.0f*A0*A1;
				float p2 = surfaceFunction.a2 - (2.0f*A0*A2 + A1*A1);
				float p3 = surfaceFunction.a3 - 2.0f*A1*A2;
				float p4 = surfaceFunction.a4 - A2*A2;
				
				// Solve the quartic polynomial:
				List<float> intersections = QuarticFunction.Solve(p0, p1, p2, p3, p4);
				//TODO: The previous Solve() function has a constant value in the last parameter,
				// but that is wrong. It should be `p4`, except since `p4` is so small, the near divide-by-zero
				// will cause a numeric instability and cause the results to fluctuate. This resolves it somewhat,
				// but proper solution MUST be implemented before this code is production-ready.
				Console.WriteLine("p4: " + p4);
				
				// Only return the value if it is sampled within the segment that we are currently considering,
				// otherwise the value we got is invalid:
				foreach (var j in intersections)
				{
					if (((z0 + c*j) > x1) && ((z0 + c*j) <= x2))
					{
						output.Add(j);
					}
				}
			}
			
			return output;
		}
		
	}
}
