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
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
    public class MutableCubicSpline1D : MutableCurve<Mutable2DPoint, Real>
    {
        public MutableCubicSpline1D() : base(new CubicSpline1DFactory(), new Mutable2DPointFactory())
        {
        }

        private class CubicSpline1DFactory : ICurveFactory<Mutable2DPoint, Real>
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

        private class CubicSpline1DAdapter : ICurve<Real>
        {
            private CubicSpline1D _spline;

            public CubicSpline1DAdapter(CubicSpline1D spline)
            {
                _spline = spline;
            }


            public Real GetValueAt(Real x) => new Real(_spline.GetValueAt((float)x));

            public Real GetDerivativeAt(Real x, uint derivative) =>
                new Real(_spline.GetNthDerivativeAt((float)x, derivative));
        }
    }
}
