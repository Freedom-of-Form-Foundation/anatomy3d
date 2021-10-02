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

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{

    /// <summary>
    /// MutablePiecewiseInterpolatedCurve represents a curve of some shape defined by a collection of some variety of control points, which
    /// can be moved during the course of the program. It is safe to update different points concurrently and
    /// safe to read concurrently, but it is not safe to read while updating points. Control points must issue
    /// a callback when their data changes to provoke this class to recalculate its curve.
    /// </summary>
    /// <typeparam name="TControl">Control point type to define the curve.</typeparam>
    /// <typeparam name="TOut">Point type for the output curve. (This will usually be an immutable type.)</typeparam>
    public class MutablePiecewiseInterpolatedCurve<TControl, TOut>:ICurve<TOut>,IEnumerable<TControl> where TControl:class
    {
        private readonly ICurveFactory<TControl, TOut> _curveFactory;
        private readonly IMutablePointFactory<TControl> _pointFactory;
        private readonly List<TControl> _points;
        private readonly object _curveLock;
        private ICurve<TOut> _currentCurve;

        ///<summary>
        /// Construct a MutablePiecewiseInterpolatedCurve, using a provided algorithm to calculate curves (given a snapshot of control points
        /// at an instant in time) and to create points (from callbacks to use when modified).
        /// </summary>
        /// <param name="curveFactory">Algorithm for calculating immutable ICurve{TOut} curves from a collection of
        /// TControl points at one specific point in time.</param>
        /// <param name="pointFactory">Factory for TControl points.</param>
        public MutablePiecewiseInterpolatedCurve(ICurveFactory<TControl, TOut> curveFactory, IMutablePointFactory<TControl> pointFactory)
        {
            _curveFactory = curveFactory ?? throw new ArgumentNullException(nameof(curveFactory));
            _pointFactory = pointFactory ?? throw new ArgumentNullException(nameof(pointFactory));
            _curveLock = new object();
            _points = new List<TControl>();
        }


        /// <summary>
        /// Add a new point to this curve. It starts at a default location. When it is moved, this structure will be
        /// notified to update its calculated curve.
        /// </summary>
        /// <returns>A new TControl from the point factory, which is now part of this curve. It is appended to the
        /// internal point list, so it has the (new) highest index.</returns>
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

        /// <summary>
        /// Find a specific point object and remove it from this curve.
        /// </summary>
        /// <param name="p">Object to remove.</param>
        /// <returns>Whether this object was found and removed.</returns>
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

        /// <summary>
        /// Remove a point object by index from this curve.
        /// </summary>
        /// <param name="index">Index of the point to remove.</param>
        public void RemoveAt(int index)
        {
            lock (_curveLock)
            {
                _points.RemoveAt(index);
                _currentCurve = null;
            }
        }

        /// <summary>
        /// The number of points currently in this curve.
        /// </summary>
        public int Count => _points.Count;

        /// <summary>
        /// Get a point by index on this curve. Indexes start at 0, are assigned sequentially to new points, and
        /// if a point is removed, points ahead of the removed point have their indices reduced.
        /// </summary>
        /// <param name="index">Index of the point to fetch.</param>
        public TControl this[int index] => _points[index];

        /// <summary>
        /// Get an ICurve{TOut} corresponding to the current location of the points in this curve.
        /// </summary>
        /// <returns>An ICurve{TOut} from _curveFactory, from the current points.</returns>
        public ICurve<TOut> CurrentCurve()
        {
            lock (_curveLock)
            {
                // This uses the lazily-evaluated null-coalescence operator to either return the curve we have
                // or, if we don't have one, calculate a new one, save it, and then return that.
                // We save the curves we calculate to avoid the (potentially expensive) recalculation every time
                // a single value is requested, but recalculate whenever we did not have one (either because it
                // was never calculated before, or because we dropped it since a point moved and we need to
                // recalculate the curve next time something wants it - which is right now).
                return _currentCurve ?? (_currentCurve = _curveFactory.NewCurve(_points));
            }
        }

        /// <summary>
        /// Remove the currently-cached curve so it will be recalculated next time it is needed.
        /// </summary>
        private void BecomeDirty()
        {
            lock (_curveLock)
            {
                _currentCurve = null;
            }
        }

        /// <summary>
        /// Calculate the value of this curve at the provided location. (The underlying curve type defines the domain
        /// and range.)
        /// </summary>
        /// <param name="x">Location to calculate the value of on the current state of the curve.</param>
        /// <returns>The curve's value at position x.</returns>
        public TOut GetValueAt(Real x) => CurrentCurve().GetValueAt(x);

        /// <summary>
        /// Calculate the derivative of the current curve at the specified point. The underlying curve type defines the
        /// domain, range, and output in case of discontinuities or points where the derivative is undefined.
        /// </summary>
        /// <param name="x">Location on the curve to calculate a derivative.</param>
        /// <param name="derivative">Degree of derivation. 0th derivative is the function itself.</param>
        /// <returns>The value of the given derivative of the curve at location x.</returns>
        public TOut GetDerivativeAt(Real x, uint derivative) => CurrentCurve().GetDerivativeAt(x, derivative);

        /// <summary>
        /// Get an enumerator over the control points on this curve.
        /// </summary>
        /// <returns>An enumerator over the control points on this curve.</returns>
        public IEnumerator<TControl> GetEnumerator() => _points.GetEnumerator();

        /// <summary>
        /// Get an enumerator over the control points on this curve but without generic type awareness.
        /// </summary>
        /// <returns>The same as IEnumerator{TControl}.GetEnumerator().</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Interface for a factory that creates mutable points. Mutable points allow their location to be edited, but
    /// call back into the collection that contains them when they are changed. If they do not issue these callbacks,
    /// the curve may not recognize the new location of the point.
    /// </summary>
    /// <typeparam name="TMutablePoint">Mutable point type created by this factory.</typeparam>
    public interface IMutablePointFactory<out TMutablePoint> where TMutablePoint : class
    {
        /// <summary>
        /// Create a new TMutablePoint, configured to call the specific Action after the point is modified. The callback
        /// must be called after the requested change would be observable if the point is read.
        /// </summary>
        /// <param name="mutationCallback"></param>
        /// <returns>A new TMutablePoint ready to call back into mutationCallback when mutated.</returns>
        TMutablePoint NewPoint(Action mutationCallback);
    }

    /// <summary>
    /// Interface for a factory that calculates curves given the current state of the points in the MutablePiecewiseInterpolatedCurve.
    /// The output curve must have its own internal, immutable copy of the points, since the point locations may later
    /// be updated but the output curve needs to be constant.
    /// </summary>
    /// <typeparam name="TParameterPoint">Mutable control point type.</typeparam>
    /// <typeparam name="TOutputPoint">Output type for points on the curve that will be calculated.</typeparam>
    public interface ICurveFactory<in TParameterPoint, out TOutputPoint>
    {
        /// <summary>
        /// Calculate an ICurve{TOutputPoint} from the current state of the parameters.
        /// </summary>
        /// <param name="parameters">Enumerable of the parameter points on the curve. They are in the order they
        /// were created; they are not otherwise sorted.</param>
        /// <returns></returns>
        ICurve<TOutputPoint> NewCurve(IEnumerable<TParameterPoint> parameters);
    }

    /// <summary>
    /// Interface representing an arbitrary curve that calculates values in some output space given a single Real
    /// input parameter.
    /// </summary>
    /// <typeparam name="TOut">Type representing the output space of this curve.</typeparam>
    public interface ICurve<out TOut> {
        /// <summary>
        /// Calculate the value of this curve at the provided location.
        /// </summary>
        /// <param name="x">Location to calculate at.</param>
        /// <returns>Value of this curve at x.</returns>
        TOut GetValueAt(Real x);
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
        TOut GetDerivativeAt(Real x, uint derivative);
    }

    // Convenience functions for ICurve.
    public static class CurveExtensions
    {
        /// <summary>
        /// Calculate the first derivative of this curve at the provided location.
        /// <see cref="ICurve{TOut}.GetDerivativeAt"/>
        /// </summary>
        /// <param name="curve">This curve to calculate a derivative on.</param>
        /// <param name="x">Location to calculate the curve's first derivative at.</param>
        /// <typeparam name="TOut">Output point type of the curve.</typeparam>
        /// <returns>The first derivative of this curve at the specified location.</returns>
        public static TOut GetDerivativeAt<TOut>(this ICurve<TOut> curve, Real x) => curve.GetDerivativeAt(x, 1);
    }
}
