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

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	/// Interface representing an arbitrary curve that calculates values in some output space given a single double
	/// input parameter.
	/// </summary>
	/// <typeparam name="TOut">Type representing the output space of this curve.</typeparam>
	public abstract class DerivableFunction<TIn, TOut> : ContinuousMap<TIn, TOut>
	{
		/// <summary>
		/// Calculate the given derivative of this curve at the provided location. There is no standard for representing
		/// undefined derivatives; document your decisions.
		/// </summary>
		/// <remarks>If a derivative is undefined at a removable discontinuity, removing the discontinuity is probably
		/// the best choice for computer graphics rendering. If there is a defined limit of the derivative at one side,
		/// that's probably the best value. If there is a defined limit at both sides but the limit is different,
		/// pick one. If the derivative really can't be calculated, consider NaN or equivalent, but this may not
		/// render very well.</remarks>
		/// <param name="x">Location to calculate the derivative at.</param>
		/// <param name="derivative">The degree of derivative to calculate.</param>
		/// <returns>The given derivative of this curve at x.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// 	The value that is sampled must lie between or on the outermost points on which the function is defined.
		/// 	If <c>x</c> is outside that domain, an <c>ArgumentOutOfRangeException</c> is thrown.
		/// </exception>
		public abstract TOut GetNthDerivativeAt(TIn x, uint derivative);

		/// <summary>
		/// Calculate the first derivative of this curve at the provided location.
		/// <see cref="ICurve{TOut}.GetDerivativeAt"/>
		/// </summary>
		/// <param name="curve">This curve to calculate a derivative on.</param>
		/// <param name="x">Location to calculate the curve's first derivative at.</param>
		/// <typeparam name="TOut">Output point type of the curve.</typeparam>
		/// <returns>The first derivative of this curve at the specified location.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// 	The value that is sampled must lie between or on the outermost points on which the function is defined.
		/// 	If <c>x</c> is outside that domain, an <c>ArgumentOutOfRangeException</c> is thrown.
		/// </exception>
		public TOut GetDerivativeAt(TIn x) => GetNthDerivativeAt(x, 1);
	}

	public static class DerivableFunctionExtensions
	{
		private const int DefaultNewtonRaphsonIterations = 16;

		/// <summary>
		/// Gradually approach the root of a derivable function using the Newton-Raphson iterative numerical method.
		/// </summary>
		/// <param name="start">Point at which to start searching for a root. For best results, this starting point must
		/// be close to the root.</param>
		public static double NewtonRaphson(this DerivableFunction<double, double> self,
			double start,
			int iterations = DefaultNewtonRaphsonIterations)
		{
			double x = start;
			for (int i = 0; i < iterations; i++)
			{
				x = x - self.GetValueAt(x) / self.GetDerivativeAt(start);
			}
			return x;
		}
	}
}
