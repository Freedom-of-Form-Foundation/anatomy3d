using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FreedomOfFormFoundation.AnatomyEngine;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;
using Xunit;

namespace EngineTests.calculus
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

        /// <summary>
        /// AssertOnCurve asserts that this point's derivatives at location Location on the given curve are all as stored
        /// in this point. The 0th derivative is the value, and is tested both as the 0th derivative and the value.
        /// </summary>
        /// <param name="curve"></param>
        public void AssertOnCurve(ICurve<Real> curve)
        {
            if (Derivatives.Count == 0)
            {
                Assert.Equal(0, curve.GetValueAt(X));
                Assert.Equal(0, curve.GetDerivativeAt(X));
                foreach (int d in Enumerable.Range(0, 10))
                {
                    Assert.Equal(0, curve.GetDerivativeAt(X, (uint)d));
                }
                return;
            }

            Assert.Equal(Derivatives[0], curve.GetValueAt(X));
            for (uint d = 0; d < Derivatives.Count; ++d)
            {
                Assert.Equal(Derivatives[(int)d], curve.GetDerivativeAt(X, d));
            }
        }
    }

    /// <summary>
    /// A mutable wrapper for a LiteralResultPoint, following MutablePiecewiseInterpolatedCurve control point rules of issuing a callback
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

        public static MutablePiecewiseInterpolatedCurve<MutableLiteralResultPoint, Real> NewMutable()
        {
            return new MutablePiecewiseInterpolatedCurve<MutableLiteralResultPoint, Real>(new LiteralCurveFactory(),
                new MutableLiteralResultPointFactory());
        }
    }

    public class LiteralCurveFactory : ICurveFactory<MutableLiteralResultPoint, Real>
    {
        public ICurve<Real> NewCurve(IEnumerable<MutableLiteralResultPoint> parameters) => new LiteralCurve(parameters);
    }

    public class MutablePiecewiseInterpolatedCurveTests
    {
        private static readonly Random Rand = new Random();
        public static IEnumerable<object[]> RandomPointLists() => from pointCount in Enumerable.Range(1, 25) select new object[] {RandomPoints(pointCount)};

        private static IEnumerable<LiteralResultPoint> RandomPoints(int pointCount) => from _ in Enumerable.Range(0, pointCount) select RandomPoint();

        private static LiteralResultPoint RandomPoint() =>  new LiteralResultPoint
        {
            X = new Real(Rand.NextDouble()),
            Derivatives = new List<Real>(from discard2 in Enumerable.Range(0, Rand.Next(1, 25))
                select new Real(Rand.Next(-10_000_000, 10_000_000)))
        };

        [Theory]
        [MemberData(nameof(RandomPointLists))]
        public void TestUnchangingCurveWithPoints(IEnumerable<LiteralResultPoint> pointsEnumerable)
        {
            var points = new List<LiteralResultPoint>(pointsEnumerable);
            var mutableCurve = LiteralCurve.NewMutable();
            foreach (var p in points)
            {
                var mutablePoint = mutableCurve.NewPoint();
                mutablePoint.CompletePoint = p;
            }

            foreach (var p in points)
            {
                p.AssertOnCurve(mutableCurve);
            }

            ICurve<Real> fixedCurve = mutableCurve.CurrentCurve();
            foreach (var p in points)
            {
                p.AssertOnCurve(fixedCurve);
            }
        }

        [Theory]
        [MemberData(nameof(RandomPointLists))]
        public void TestChangingCurveWithPoints(IEnumerable<LiteralResultPoint> pointsEnumerable)
        {
            var origPoints = new List<LiteralResultPoint>(pointsEnumerable);
            var mutablePoints = new List<MutableLiteralResultPoint>(origPoints.Count);
            var mutableCurve = LiteralCurve.NewMutable();
            foreach (var p in origPoints)
            {
                var mutablePoint = mutableCurve.NewPoint();
                mutablePoint.CompletePoint = p;
                mutablePoints.Add(mutablePoint);
            }

            ICurve<Real> firstCurve = mutableCurve.CurrentCurve();
            foreach (var p in origPoints)
            {
                p.AssertOnCurve(firstCurve);
                p.AssertOnCurve(mutableCurve);
            }

            var replacementPoints = new List<LiteralResultPoint>(mutablePoints.Zip(RandomPoints(mutablePoints.Count),
                (mutablePoint, newValue) => mutablePoint.CompletePoint = newValue));

            // The new curve, and the current state of the mutable curve, should represent the new points.
            foreach (var p in replacementPoints)
            {
                p.AssertOnCurve(mutableCurve);
            }
            ICurve<Real> secondCurve = mutableCurve.CurrentCurve();
            foreach (var p in replacementPoints)
            {
                p.AssertOnCurve(secondCurve);
            }

            // But the original curve should be unchanged.
            foreach (var p in origPoints)
            {
                p.AssertOnCurve(firstCurve);
            }
        }
    }
}
