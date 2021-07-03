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
	///     Class <c>QuadraticFunction</c> describes a quadratic function defined by \f$q(x) = a_0 + a_1 x + a_2 x^2\f$.
	/// </summary>
	public class QuadraticFunction : ContinuousMap<float, float>
	{
		float a0;
		float a1;
		float a2;
		
		/// <summary>
		///     A quadratic function defined by \f$q(x) = a_0 + a_1 x + a_2 x^2\f$.
		/// </summary>
		public QuadraticFunction(float a0, float a1, float a2)
		{
			this.a0 = a0;
			this.a1 = a1;
			this.a2 = a2;
		}
		
		public float GetAt(float x, uint derivative)
		{
			// Return a different function depending on the derivative level:
			switch (derivative)
			{
				case 0: return a0 + a1*x + a2*x*x;
				case 1: return a1 + 2.0f*a2*x;
				case 2: return 2.0f*a2;
				default: return 0.0f;
			}
		}
		
		/// <inheritdoc />
		public override float GetValueAt(float x)
		{
			return GetAt(x, 0);
		}
		
		public float GetDerivativeAt(float x)
		{
			return GetAt(x, 1);
		}
		
		/// <summary>
		///     Solves the equation \f$a_0 + a_1 x + a_2 x^2 = 0\f$, returning all real values of
		///		\f$x\f$ for which the equation is true using the 'abc-formula' using the parameters
		/// 	in this instance of QuadraticFunction.
		/// </summary>
		public IEnumerable<float> Roots()
		{
			return QuadraticFunction.Solve(a0, a1, a2);
		}
		
		/// <summary>
		///     Solves a general equation \f$a_0 + a_1 x + a_2 x^2 = 0\f$, returning all real values of
		///		\f$x\f$ for which the equation is true using the 'abc-formula'.
		/// </summary>
		public static IEnumerable<float> Solve(float a0, float a1, float a2)
		{
			if(Math.Abs(a2) <= 0.005f)
			{
				if(Math.Abs(a1) <= 0.005f)
				{
					// There are no roots found:
					yield break;
				} else {
					// There is a single root, found from solving the linear equation with a1=0:
					yield return -a0/a1;
					
				}
				yield break;
			}
			
			float x1 = 0.5f*(-a1 + (float)Math.Sqrt(a1*a1 - 4.0f*a0*a2))/a2;
			float x2 = 0.5f*(-a1 - (float)Math.Sqrt(a1*a1 - 4.0f*a0*a2))/a2;
			
			if(Single.IsNaN(x1) == false)
			{
				yield return x1;
			}
			
			if(Single.IsNaN(x2) == false)
			{
				yield return x2;
			}
		}
	}
}
