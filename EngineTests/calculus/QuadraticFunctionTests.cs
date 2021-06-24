using System;
using System.Reflection;
using FreedomOfFormFoundation.AnatomyEngine;
using Xunit;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;
using Xunit.Abstractions;

namespace EngineTests.calculus
{
    public class QuadraticFunctionTests
    {
        // Utility for associating logged output with a test.
        private readonly ITestOutputHelper testLogger;

        public QuadraticFunctionTests(ITestOutputHelper testLogger)
        {
            this.testLogger = testLogger;
        }

        /// <summary>
        /// Helper method: are these numeric values relatively near each other?
        ///
        /// This is pretty much just wrapping a cast to Real and using its IsRelativelyNear algorithm.
        ///
        /// Developer note: 0 isn't "relatively near" anything else because no amount of nonzero multiplication will get
        /// a nonzero value to zero. This is part of why I'm using "relatively near" here - I think unexpected
        /// transitions into or out of 0 will cause discontinuities and errors.
        /// </summary>
        /// <param name="a">First value to compare.</param>
        /// <param name="b">Second value to compare.</param>
        /// <param name="tolerance">Proportional error allowed to still consider the elements near each other.</param>
        /// <returns>Whether the proportional difference between a and b is within the tolerance.</returns>
        private bool Near(Real a, Real b, Real tolerance)
        {
            bool ret = a.IsRelativelyNear(b, tolerance);
            if (!ret)
            {
                testLogger.WriteLine(
                    "Impending test failure: {0} is not near {1} (tolerance {2})",
                    a, b, tolerance
                );
            }

            return ret;
        }

        /// <summary>
        /// Helper method: are these numeric values proportionally within 1e-5 of each other?
        ///
        /// Developer note: If 1e-5 turns out to be a bad default tolerance, change it, it's vaguely arbitrary.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private bool Near(Real a, Real b)
        {
            return Near(a, b, 1e-5);
        }

        /// <summary>
        /// Tests quadratic function evaluation in cases where the expected value is a number. This test passes if the
        /// actual result is relatively near the expected result
        /// (<see cref="Near(FreedomOfFormFoundation.AnatomyEngine.Real,FreedomOfFormFoundation.AnatomyEngine.Real)"/>
        /// for how near "relatively near" is).
        /// </summary>
        /// <param name="a0">Constant term in quadratic function.</param>
        /// <param name="a1">Linear term in quadratic function.</param>
        /// <param name="a2">Quadratic term in quadratic function.</param>
        /// <param name="x">Value to test at.</param>
        /// <param name="expected"></param>
        [Theory]
        [InlineData(0f, 0f, 0f, 0f, 0f)]
        [InlineData(1000f, 0f, 0f, 0f, 1000f)]
        [InlineData(1000f, 0f, 0f, 87429f, 1000f)]
        [InlineData(0f, 1f, 0f, 0f, 0f)]
        [InlineData(0f, 1f, 0f, 3.14159f, 3.14159f)]
        [InlineData(0f, 1e10f, 0f, 1e5f, 1e15f)]
        [InlineData(42f, 20f, 0f, 10f, 242f)]
        [InlineData(float.NegativeInfinity, 1f, 0f, 10f, float.NegativeInfinity)]
        [InlineData(float.PositiveInfinity, 1e-15f, 0f, float.PositiveInfinity, float.PositiveInfinity)]
        [InlineData(1000f, 0f, 0f, float.NegativeInfinity, float.NegativeInfinity)]
        public void TestValueCalculation(float a0, float a1, float a2, float x, float expected)
        {
            QuadraticFunction q = new QuadraticFunction(a0, a1, a2);
            Assert.NotNull(q);
            Assert.True(Near(expected, q.GetValueAt(x)));
            Assert.True(Near(expected, q.GetAt(x, 0)));
        }

        /// <summary>
        /// Tests quadratic function evaluation in cases where the expected value is NaN, which
        /// requires special comparison.
        ///
        /// Note that, in IEEE floating point, 0 is "zero-ish", so multiplying infinity by a zero is numerically
        /// undefined, represented as NaN. Cases multiplying infinity by zero wind up here.
        /// </summary>
        /// <param name="a0">Constant term in quadratic function.</param>
        /// <param name="a1">Linear term in quadratic function.</param>
        /// <param name="a2">Quadratic term in quadratic function.</param>
        /// <param name="x">Value to test at.</param>
        [Theory]
        [InlineData(float.PositiveInfinity, 1e-15f, 0f, float.NegativeInfinity)]
        public void TestValueNaN(float a0, float a1, float a2, float x)
        {
            QuadraticFunction q = new QuadraticFunction(a0, a1, a2);
            Assert.NotNull(q);
            Assert.True(float.IsNaN(q.GetValueAt(x)));
            Assert.True(float.IsNaN(q.GetAt(x, 0)));
        }

        /// <summary>
        /// Tests evaluation of a function at derivatives from 0 to 4. Does not support expected
        /// NaN results. The 3rd and 4th derivatives of a quadratic function are always 0, so
        /// there is no parameter for this.
        /// </summary>
        /// <param name="a0">Constant term in the quadratic function.</param>
        /// <param name="a1">Linear term in the quadratic function.</param>
        /// <param name="a2">Quadratic term in the quadratic function.</param>
        /// <param name="x">Value of X to test at.</param>
        /// <param name="atD0">Expected value of the function (the 0th derivative) at x.</param>
        /// <param name="atD1">Expected value of the first derivative at x.</param>
        /// <param name="atD2">Expected value of the second derivative at x.</param>
        [Theory]
        [InlineData(0f,0f,0f,0f,0f,0f,0f)]
        [InlineData(1f, 0f, 0f, 20f, 1f, 0f, 0f)]
        [InlineData(1f, 2f, 0f, 10f, 21f, 2f, 0f)]
        [InlineData(1f, 2f, 4f, 2f, 21f, 18f, 8f)]
        public void TestDerivativesAt(float a0, float a1, float a2, float x, float atD0, float atD1, float atD2)
        {
            QuadraticFunction q = new QuadraticFunction(a0, a1, a2);
            Assert.NotNull(q);
            Assert.True(Near(q.GetAt(x, 0), atD0));
            Assert.True(Near(q.GetAt(x, 1), atD1));
            Assert.True(Near(q.GetDerivativeAt(x), atD1));
            Assert.True(Near(q.GetAt(x, 2), atD2));
            Assert.Equal(q.GetAt(x, 3), 0);
            Assert.Equal(q.GetAt(x, 4), 0);
        }
    }
}
