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
using System.Diagnostics;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	///     Class <c>QuadraticFunction</c> describes a quadratic function defined by \f$q(x) = a_0 + a_1 x + a_2 x^2\f$.
	/// </summary>
	public class QuadraticFunction : ContinuousMap<double, double>
	{
		private readonly double _a0;
		private readonly double _a1;
		private readonly double _a2;
		
		/// <summary>
		///     A quadratic function defined by \f$q(x) = a_0 + a_1 x + a_2 x^2\f$.
		/// </summary>
		public QuadraticFunction(double a0, double a1, double a2)
		{
			DebugUtil.AssertFinite(a0, nameof(a0));
			DebugUtil.AssertFinite(a1, nameof(a1));
			DebugUtil.AssertFinite(a2, nameof(a2));
			_a0 = a0;
			_a1 = a1;
			_a2 = a2;
		}
		
		public double GetNthDerivativeAt(double x, uint derivative)
		{
			DebugUtil.AssertFinite(x, nameof(x));
			// Return a different function depending on the derivative level:
			switch (derivative)
			{
				case 0: return _a0 + _a1*x + _a2*x*x;
				case 1: return _a1 + 2.0f*_a2*x;
				case 2: return 2.0f*_a2;
				default: return 0.0f;
			}
		}
		
		/// <inheritdoc />
		public override double GetValueAt(double x)
		{
			return GetNthDerivativeAt(x, 0);
		}
		
		public double GetDerivativeAt(double x)
		{
			return GetNthDerivativeAt(x, 1);
		}
		
		/// <summary>
		///     Solves the equation \f$a_0 + a_1 x + a_2 x^2 = 0\f$, returning all real values of
		///		\f$x\f$ for which the equation is true using the 'abc-formula' using the parameters
		/// 	in this instance of QuadraticFunction.
		/// </summary>
		public IEnumerable<double> Roots()
		{
			return Solve(_a0, _a1, _a2);
		}
		
		/// <summary>
		///     Solves a general equation \f$a_0 + a_1 x + a_2 x^2 = 0\f$, returning all real values of
		///		\f$x\f$ for which the equation is true using the 'abc-formula'.
		/// </summary>
		public static IEnumerable<double> Solve(double a0, double a1, double a2)
		{
			DebugUtil.AssertFinite(a0, nameof(a0));
			DebugUtil.AssertFinite(a1, nameof(a1));
			DebugUtil.AssertFinite(a2, nameof(a2));
			if(Math.Abs(a2) <= 0.005)
			{
				if(Math.Abs(a1) <= 0.005)
				{
					// There are no roots found:
					yield break;
				} else {
					// There is a single root, found from solving the linear equation with a1=0:
					yield return -a0/a1;
					
				}
				yield break;
			}

			double x1 = 0.5*(-a1 + Math.Sqrt(a1*a1 - 4.0*a0*a2))/a2;
			double x2 = 0.5*(-a1 - Math.Sqrt(a1*a1 - 4.0*a0*a2))/a2;
			
			if(Double.IsNaN(x1) == false)
			{
				yield return x1;
			}

			if(Double.IsNaN(x2) == false)
			{
				yield return x2;
			}
		}
	}
}
