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
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	/// A CubicFunction defines a cubic function defined by \f$q(x) = a_0 + a_1 x + a_2 x^2 + a_3 x^3\f$.
	/// </summary>
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
		
		public float GetNthDerivativeAt(float x, uint derivative)
		{
			// Return a different function depending on the derivative level:
			switch (derivative)
			{
				case 0: return a0 + a1*x + a2*x*x + a3*x*x*x;
				case 1: return a1 + 2.0f*a2*x + 3.0f*a3*x*x;
				case 2: return 2.0f*a2 + 6.0f*a3*x;
				case 3: return 6.0f*a3;
				default: return 0.0f;
			}
		}
		
		public override float GetValueAt(float x)
		{
			return GetNthDerivativeAt(x, 0);
		}
		
		public float GetDerivativeAt(float x)
		{
			return GetNthDerivativeAt(x, 1);
		}
		
		public IEnumerable<float> Roots()
		{
			return CubicFunction.Solve(a0, a1, a2, a3);
		}
		
		/// <summary>
		///     Solves the equation \f$d + c x + b x^2 + a x^3 = 0\f$, returning all real values of
		///		\f$x\f$ for which the equation is true. See https://en.wikipedia.org/wiki/Cubic_equation for the
		///		algorithm used.
		/// </summary>
		public static IEnumerable<float> Solve(float d, float c, float b, float a)
		{
			if(Math.Abs(a) <= 0.005f)
			{
				foreach (float v in QuadraticFunction.Solve(d, c, b))
				{
					yield return v;
				}
				yield break;
			}
			
			double delta0 = b*b - 3.0*a*c;
			double delta1 = 2.0*b*b*b - 9.0*a*b*c + 27.0*a*a*d;
			
			Complex p1 = Complex.Sqrt(delta1*delta1 - 4.0*delta0*delta0*delta0);
			
			// The sign we choose in the next equation is arbitrary. To prevent a divide-by-zero down the line, if p2 is
			// zero, we must choose the opposite sign to make it nonzero:
			Complex p2 = delta1 + p1;
			
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
					yield return (float)root.Real;
				}
			}
		}
	}
}
