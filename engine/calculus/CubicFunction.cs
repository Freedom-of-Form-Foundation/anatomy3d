using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	public class CubicFunction : ContinuousMap<float, float>
	{
		float a0;
		float a1;
		float a2;
		float a3;
		
		/// <summary>
		///     A cubic function defined by \f$q(x) = a_0 + a_1 x + a_2 x^2 + a_3 x^3\f$.
		///		See https://en.wikipedia.org/wiki/Cubic_equation for more information.
		/// </summary>
		public CubicFunction(float a0, float a1, float a2, float a3)
		{
			this.a0 = a0;
			this.a1 = a1;
			this.a2 = a2;
			this.a3 = a3;
		}
		
		public float GetAt(float x, uint derivative)
		{
			// Return a different function depending on the derivative level:
			float output = 0.0f;
			switch (derivative)
			{
				case 0:
					output = a0 + a1*x + a2*x*x + a3*x*x*x;
					break;
				case 1:
					output = a1 + 2.0f*a2*x + 3.0f*a3*x*x;
					break;
				case 2:
					output = 2.0f*a2 + 6.0f*a3*x;
					break;
				case 3:
					output = 6.0f*a3;
					break;
				default:
					output = 0.0f;
					break;
			}
			
			return output;
		}
		
		public override float GetValueAt(float x)
		{
			return GetAt(x, 0);
		}
		
		public float GetDerivativeAt(float x)
		{
			return GetAt(x, 1);
		}
		
		public List<float> Roots()
		{
			return CubicFunction.Solve(a0, a1, a2, a3);
		}
		
		/// <summary>
		///     Solves the equation \f$d + c x + b x^2 + a x^3 = 0\f$, returning all real values of
		///		\f$x\f$ for which the equation is true. See https://en.wikipedia.org/wiki/Cubic_equation for the
		///		algorithm used.
		/// </summary>
		public static List<float> Solve(float d, float c, float b, float a)
		{
			if(Math.Abs(a) <= 0.005f)
			{
				return QuadraticFunction.Solve(d, c, b);
			}
			
			List<float> output = new List<float>(4);
			
			double delta0 = b*b - 3.0*a*c;
			double delta1 = 2.0*b*b*b - 9.0*a*b*c + 27.0*a*a*d;
			
			Complex p1 = Complex.Sqrt(delta1*delta1 - 4.0*delta0*delta0*delta0);
			
			// The sign we choose in the next equation is arbitrary. To prevent a divide-by-zero down the line, if p2 is
			// zero, we must choose the opposite sign to make it nonzero:
			Complex p2 = delta1 + p1;
			//Complex p2 = delta1 - p1;
			
			Complex C = Complex.Pow(0.5*p2, (1.0/3.0));
			
			Complex xi = -0.5 + 0.5*Complex.Sqrt(-3.0);
			
			List<Complex> roots = new List<Complex>
			{
				-1.0/(3.0*a)*(b + C + delta0/C),
				-1.0/(3.0*a)*(b + xi*C + delta0/(xi*C)),
				-1.0/(3.0*a)*(b + xi*xi*C + delta0/(xi*xi*C)),
			};
			
			foreach (Complex root in roots)
			{
				if(Math.Abs(root.Imaginary) <= 0.05f)
				{
					output.Add((float)root.Real);
				}
			}
			
			return output;
		}
	}
}
