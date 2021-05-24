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
			float output = 0.0f;
			switch (derivative)
			{
				case 0:
					output = a0 + a1*x + a2*x*x;
					break;
				case 1:
					output = a1 + 2.0f*a2*x;
					break;
				case 2:
					output = 2.0f*a2;
					break;
				default:
					output = 0.0f;
					break;
			}
			
			return output;
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
		public List<float> Roots()
		{
			return QuadraticFunction.Solve(a0, a1, a2);
		}
		
		/// <summary>
		///     Solves a general equation \f$a_0 + a_1 x + a_2 x^2 = 0\f$, returning all real values of
		///		\f$x\f$ for which the equation is true using the 'abc-formula'.
		/// </summary>
		public static List<float> Solve(float a0, float a1, float a2)
		{
			if(Math.Abs(a2) <= 0.005f)
			{
				if(Math.Abs(a1) <= 0.005f)
				{
					return new List<float>(0);
				} else {
					return new List<float>{-a0/a1};
				}
			}
			
			List<float> output = new List<float>(2);
			
			float x1 = 0.5f*(-a1 + (float)Math.Sqrt(a1*a1 - 4.0f*a0*a2))/a2;
			float x2 = 0.5f*(-a1 - (float)Math.Sqrt(a1*a1 - 4.0f*a0*a2))/a2;
			
			if(Single.IsNaN(x1) == false)
			{
				output.Add(x1);
			}
			
			if(Single.IsNaN(x2) == false)
			{
				output.Add(x2);
			}
			
			return output;
		}
	}
}
