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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
    public class FlexSpline : ContinuousMap<Real, Real>, IEnumerable<MovingPoint2D>
    {
        private List<MovingPoint2D> _points;

        // recalculate on point change
        private object _splineLock;
        private CubicSpline1D _calculatedSpline;

        private void BecomeDirty()
        {
            lock (_splineLock)
            {
                _calculatedSpline = null;
            }
        }

        public CubicSpline1D CurrentSpline()
        {
            lock (_splineLock)
            {
                if (_calculatedSpline != null)
                {
                    return _calculatedSpline;
                }

                _points.Sort();
                SortedList<float, float> sl = new SortedList<float, float>();
                foreach (MovingPoint2D p in _points)
                {
                    sl.Add((float)p.X, (float)p.Y);
                }

                _calculatedSpline = new CubicSpline1D(sl);
                return _calculatedSpline;
            }
        }

        public FlexSpline()
        {
            _splineLock = new object();
            _points = new List<MovingPoint2D>();
            _calculatedSpline = null;
        }

        public MovingPoint2D NewPoint(Real x, Real y)
        {
            lock (_splineLock)
            {
                _calculatedSpline = null;
                MovingPoint2D p = new MovingPoint2D(BecomeDirty, x, y);
                _points.Add(p);
                return p;
            }
        }

        public MovingPoint2D NewPoint() => NewPoint(0, 0);

        public bool RemovePoint(MovingPoint2D p)
        {
            lock (_splineLock)
            {
                _calculatedSpline = null;
                return _points.Remove(p);
            }
        }

        public IEnumerator<MovingPoint2D> GetEnumerator() => _points.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override Real GetValueAt(Real x) => CurrentSpline().GetValueAt((float)x);

        public Real GetNthDerivativeAt(Real x, uint derivative) =>
            CurrentSpline().GetNthDerivativeAt((float)x, derivative);

        public Real GetDerivativeAt(Real x) => GetNthDerivativeAt(x, 1);

    }
}
