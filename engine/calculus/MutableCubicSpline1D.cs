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
using System.Linq;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
    public class MutableCubicSpline1D : MutablePiecewiseInterpolatedCurve<MutablePair, double>
    {
        /// <summary>
        /// Build a MutableCubicSpline1D with, initially, no points. (Attempts to evaluate the curve will throw
        /// exceptions until the instance is less pointless.)
        /// </summary>
        public MutableCubicSpline1D() : base(new CubicSpline1DFactory(), new MutablePairFactory())
        {
        }

        public MutablePair NewPoint(double location, double value)
        {
            MutablePair mp = base.NewPoint();
            mp.Set(location, value);
            return mp;
        }

		/// <summary>
		/// Algorithm for converting a sequence of MutablePair instances at one point in time into an immutable
		/// CubicSpline1D.
		/// </summary>
		private struct CubicSpline1DFactory : IDerivableFunctionFactory<MutablePair, double>
		{
			public DerivableFunction<double, double> NewCurve(IEnumerable<MutablePair> parameters)
			{
				SortedList<double, double> sortedPoints = new SortedList<double, double>();
				foreach (var point in parameters.OrderBy(point => point.Location))
				{
					sortedPoints.Add(point.Location, point.Value);
				}

				return new CubicSpline1D(sortedPoints);
			}
		}
	}
}
