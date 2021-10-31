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
using System.Diagnostics;
using System.Linq;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
    public class MutableCubicSpline1D : MutablePiecewiseInterpolatedCurve<MutablePair, double>
    {
        /// <summary>
        /// Build a MutableCubicSpline1D with, initially, no points. (Attempts to evaluate the curve will throw
        /// exceptions until the instance is less pointless.)
        /// </summary>
        public MutableCubicSpline1D() : base(new CubicSpline1DFactory(), new Mutable2DPointFactory())
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
		/// CubicSpline1D. Because CubicSpline1D operates on `float`, but this type declares double (because we're
		/// planning to migrate), we introduce CubicSpline1DAdapter to marshal in and out of double.
		/// </summary>
		private struct CubicSpline1DFactory : ICurveFactory<MutablePair, double>
        {
            public ICurve<double> NewCurve(IEnumerable<MutablePair> parameters)
            {
                SortedList<double, double> sortedPoints = new SortedList<double, double>();
                foreach (var point in parameters.OrderBy(point => point.Location))
                {
                    sortedPoints.Add((double)point.Location, (double)point.Value);
                }

                CubicSpline1D spline = new CubicSpline1D(sortedPoints);
                return new CubicSpline1DAdapter(spline);
            }
        }

		/// <summary>
		/// ICurve{double} view of CubicSpline1D, which doesn't exactly match the ICurve interface and - if it did -
		/// would be an ICurve{Float} anyway. This wrapper implements the interface by calling the equivalent functions
		/// on the underlying CubicSpline1D, marshalling between double and float as required.
		/// </summary>
		private struct CubicSpline1DAdapter : ICurve<double>
        {
            private CubicSpline1D _spline;

            /// <summary>
            /// Construct a CubicSpline1DAdapter wrapping a specific spline.
            /// </summary>
            /// <param name="spline">Spline to make conform to ICurve.</param>
            public CubicSpline1DAdapter(CubicSpline1D spline)
            {
                Debug.Assert(!(spline is null));
                _spline = spline;
            }

			public double GetValueAt(double x) => _spline.GetValueAt((float)x);

            public double GetDerivativeAt(double x, uint derivative) =>
                _spline.GetNthDerivativeAt((float)x, derivative);
		}
	}
}
