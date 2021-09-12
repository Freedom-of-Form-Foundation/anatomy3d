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
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
    /// <summary>
    /// MutableCubicSpline1D uses MutableCurve to implement moveable point behavior for CubicSpline1D.
    /// </summary>
    public class MutableCubicSpline1D : MutableCurve<Mutable2DPoint, Real>
    {
        /// <summary>
        /// Build a MutableCubicSpline1D with, initially, no points. (Attempts to evaluate the curve will throw
        /// exceptions until the instance is less pointless.)
        /// </summary>
        public MutableCubicSpline1D() : base(new CubicSpline1DFactory(), new Mutable2DPointFactory())
        {
        }

        /// <summary>
        /// Algorithm for converting a sequence of Mutable2DPoint instances at one point in time into an immutable
        /// CubicSpline1D. Because CubicSpline1D operates on `float`, but this type declares Real (because we're
        /// planning to migrate), we introduce CubicSpline1DAdapter to marshal in and out of Real.
        /// </summary>
        private struct CubicSpline1DFactory : ICurveFactory<Mutable2DPoint, Real>
        {
            public ICurve<Real> NewCurve(IEnumerable<Mutable2DPoint> parameters)
            {
                SortedList<float, float> sortedPoints = new SortedList<float, float>();
                foreach (var point in parameters.OrderBy(point => point.X))
                {
                    sortedPoints.Add((float)point.X, (float)point.Y);
                }

                CubicSpline1D spline = new CubicSpline1D(sortedPoints);
                return new CubicSpline1DAdapter(spline);
            }
        }

        /// <summary>
        /// ICurve{Real} view of CubicSpline1D, which doesn't exactly match the ICurve interface and - if it did -
        /// would be an ICurve{Float} anyway. This wrapper implements the interface by calling the equivalent functions
        /// on the underlying CubicSpline1D, marshalling between Real and float as required.
        /// </summary>
        private struct CubicSpline1DAdapter : ICurve<Real>
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


            public Real GetValueAt(Real x) => new Real(_spline.GetValueAt((float)x));

            public Real GetDerivativeAt(Real x, uint derivative) =>
                new Real(_spline.GetNthDerivativeAt((float)x, derivative));
        }
    }
}
