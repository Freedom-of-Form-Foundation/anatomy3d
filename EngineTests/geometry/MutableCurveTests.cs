using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FreedomOfFormFoundation.AnatomyEngine;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace EngineTests.geometry
{

    /// <summary>
    /// A type to store the values for a test LiteralCurve at one specific point. Derivatives[0] is value of the curve
    /// at the point, there's no separate storage for that. A valid LiteralResultPoint has a Derivatives list of
    /// some length, which may be 0; it is inferred to have an infinite supply of zeroes off the end. An invalid
    /// LiteralResultPoint has nil Derivatives; the zero value of LiteralResultPoint is therefore invalid.
    /// </summary>
    public struct LiteralResultPoint
    {
        public Real X;
        public List<Real> Derivatives;

        public bool IsValid => !(Derivatives is null);
    }

    /// <summary>
    /// A mutable wrapper for a LiteralResultPoint, following MutableCurve control point rules of issuing a callback
    /// immediately after being modified (when any call to an accessor that happens after the callback starts would
    /// observe the new values).
    /// </summary>
    public class MutableLiteralResultPoint
    {
        private LiteralResultPoint _p;
        private Action _mutationCallback;

        // Construct a valid MutableLiteralResultPoint representing (0, 0) with all derivatives also 0.
        public MutableLiteralResultPoint(Action mutationCallback)
        {
            _p = new LiteralResultPoint { X = 0, Derivatives = new List<Real>() };
            _mutationCallback = mutationCallback;
        }

        public LiteralResultPoint CompletePoint
        {
            get => _p;
            set
            {
                _p = value;
                _mutationCallback();
            }
        }

        public Real X
        {
            get => _p.X;
            set
            {
                _p.X = value;
                _mutationCallback();
            }
        }

        /// <summary>
        /// The indexer for MutableLiteralResultPoint reads and writes the point's derivatives. The point is modeled
        /// as having an infinite supply of zeroes in its Derivatives array, so reading from an index past the end
        /// yields a 0 and writing to an index past the end causes the array to get copied and extended.
        /// </summary>
        /// <param name="idx"></param>
        public Real this[int idx]
        {
            get => idx < _p.Derivatives.Count ? _p.Derivatives[idx] : 0 ;
            set
            {
                if (_p.Derivatives.Count <= idx)
                {
                    _p.Derivatives.AddRange(Enumerable.Repeat(new Real(0), idx - _p.Derivatives.Count + 1));
                }
                _p.Derivatives[idx] = value;
                _mutationCallback();
            }
        }
    }

    public class MutableLiteralResultPointFactory : IMutablePointFactory<MutableLiteralResultPoint>
    {
        public MutableLiteralResultPoint NewPoint(Action mutCallback) => new MutableLiteralResultPoint(mutCallback);
    }

    /// <summary>
    /// LiteralCurve is an ICurve defined only at specific exact points, where its value and derivatives are specified
    /// exactly by its control points. At all other points, its value and derivatives are NaN.
    /// </summary>
    public class LiteralCurve : ICurve<Real>
    {
        private Dictionary<Real, LiteralResultPoint> _points;

        public LiteralCurve(IEnumerable<LiteralResultPoint> points)
        {
            _points = new Dictionary<Real, LiteralResultPoint>();
            foreach (LiteralResultPoint p in points)
            {
                _points[p.X] = p;
            }
        }

        public LiteralCurve(IEnumerable<MutableLiteralResultPoint> points) : this(from p in points select p.CompletePoint)
        { }


        public Real GetValueAt(Real x) => GetDerivativeAt(x, 0);

        public Real GetDerivativeAt(Real x, uint derivative)
        {
            LiteralResultPoint p;
            if (!_points.TryGetValue(x, out p)) return Real.NaN;
            return derivative < p.Derivatives.Count ? p.Derivatives[(int)derivative] : 0;
        }
    }

    public class LiteralCurveFactory : ICurveFactory<MutableLiteralResultPoint, Real>
    {
        public ICurve<Real> NewCurve(IEnumerable<MutableLiteralResultPoint> parameters) =>
            new LiteralCurve(parameters);
    }

    public class MutableCurveTests
    {

    }
}
