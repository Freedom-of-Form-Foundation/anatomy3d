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
using System;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	/// A CubicFunction defines a cubic function defined by \f$q(x) = a_0 + a_1 x + a_2 x^2 + a_3 x^3\f$.
	/// </summary>
	public class CubicFunction : DerivableFunction<double, double>
	{
		double _a0;
		double _a1;
		double _a2;
		double _a3;
		
		/// <summary>
		///     A cubic function defined by \f$q(x) = a_0 + a_1 x + a_2 x^2 + a_3 x^3\f$.
		///		See https://en.wikipedia.org/wiki/Cubic_equation for more information.
		/// </summary>
		public CubicFunction(double a0, double a1, double a2, double a3)
		{
			_a0 = a0;
			_a1 = a1;
			_a2 = a2;
			_a3 = a3;
		}

		/// <inheritdoc />
		public override double GetNthDerivativeAt(double x, uint derivative)
		{
			DebugUtil.AssertFinite(x, nameof(x));
			// Return a different function depending on the derivative level:
			switch (derivative)
			{
				case 0: return _a0 + _a1*x + _a2*x*x + _a3*x*x*x;
				case 1: return _a1 + 2.0*_a2*x + 3.0*_a3*x*x;
				case 2: return 2.0*_a2 + 6.0*_a3*x;
				case 3: return 6.0*_a3;
				default: return 0.0;
			}
		}
		
		public override double GetValueAt(double x)
		{
			return GetNthDerivativeAt(x, 0);
		}
		
		public IEnumerable<double> Roots()
		{
			return Solve(_a0, _a1, _a2, _a3);
		}
		
		/// <summary>
		///     Solves the equation \f$d + c x + b x^2 + a x^3 = 0\f$, returning all real values of
		///		\f$x\f$ for which the equation is true. See https://en.wikipedia.org/wiki/Cubic_equation for the
		///		algorithm used.
		/// </summary>
		public static IEnumerable<double> Solve(double d, double c, double b, double a)
		{
			DebugUtil.AssertFinite(a, nameof(a));
			DebugUtil.AssertFinite(b, nameof(b));
			DebugUtil.AssertFinite(c, nameof(c));
			DebugUtil.AssertFinite(d, nameof(d));
			if(Math.Abs(a) <= 0.005)
			{
				foreach (double v in QuadraticFunction.Solve(d, c, b))
				{
					CubicFunction f = new CubicFunction(d, c, b, a);
					yield return f.NewtonRaphson(v);
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
				if(Math.Abs(root.Imaginary) <= 0.05)
				{
					DebugUtil.AssertFinite(root.Real, nameof(root.Real));
					yield return root.Real;
				}
			}
		}
	}
}
