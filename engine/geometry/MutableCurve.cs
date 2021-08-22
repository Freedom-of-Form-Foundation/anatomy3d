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

using System;
using System.Collections;
using System.Collections.Generic;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
    public class MutableCurve<TControl, TOut>:ICurve<TOut>,IEnumerable<TControl> where TControl:class
    {
        private ICurveFactory<TControl, TOut> _curveFactory;
        private IMutablePointFactory<TControl> _pointFactory;
        private List<TControl> _points;
        private object _curveLock;
        private ICurve<TOut> _currentCurve;

        public MutableCurve(ICurveFactory<TControl, TOut> curveFactory, IMutablePointFactory<TControl> pointFactory)
        {
            _curveFactory = curveFactory ?? throw new ArgumentNullException(nameof(curveFactory));
            _pointFactory = pointFactory ?? throw new ArgumentNullException(nameof(pointFactory));
            _curveLock = new object();
            _points = new List<TControl>();
        }


        public TControl NewPoint()
        {
            lock (_curveLock)
            {
                TControl p = _pointFactory.NewPoint(BecomeDirty);
                _points.Add(p);
                _currentCurve = null;
                return p;
            }
        }

        public bool RemovePoint(TControl p)
        {
            lock (_curveLock)
            {
                bool ret = _points.Remove(p);
                if (ret)
                {
                    _currentCurve = null;
                }
                return ret;
            }
        }

        public void RemoveAt(int index)
        {
            lock (_curveLock)
            {
                _points.RemoveAt(index);
                _currentCurve = null;
            }
        }

        public int Count => _points.Count;

        public TControl this[int index] => _points[index];

        public ICurve<TOut> CurrentCurve()
        {
            lock (_curveLock)
            {
                return _currentCurve ?? (_currentCurve = _curveFactory.NewCurve(_points));
            }
        }

        private void BecomeDirty()
        {
            lock (_curveLock)
            {
                _currentCurve = null;
            }
        }

        public TOut GetValueAt(Real x) => CurrentCurve().GetValueAt(x);

        public TOut GetDerivativeAt(Real x, uint derivative) => CurrentCurve().GetDerivativeAt(x, derivative);

        public IEnumerator<TControl> GetEnumerator() => _points.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public interface IMutablePointFactory<TMutablePoint>
    {
        TMutablePoint NewPoint(Action mutationCallback);
    }

    public interface ICurveFactory<TParameterPoint, TOutputPoint>
    {
        ICurve<TOutputPoint> NewCurve(IEnumerable<TParameterPoint> parameters);
    }

    public interface ICurve<TOut> {
        TOut GetValueAt(Real x);
        TOut GetDerivativeAt(Real x, uint derivative);
    }

    public static class CurveExtensions
    {
        public static TOut GetDerivativeAt<TOut>(this ICurve<TOut> curve, Real x) => curve.GetDerivativeAt(x, 1);
    }
}
