using System;
using FreedomOfFormFoundation.AnatomyEngine;
using Xunit;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace EngineTests.calculus
{
    public class QuadraticFunctionTests
    {
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
        private static bool Near(Real a, Real b, Real tolerance)
        {
            return a.IsRelativelyNear(b, tolerance);
        }

        /// <summary>
        /// Helper method: are these numeric values proportionally within 1e-5 of each other?
        ///
        /// Developer note: If 1e-5 turns out to be a bad default tolerance, change it, it's vaguely arbitrary.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool Near(Real a, Real b)
        {
            return Near(a, b, 1e-5);
        }

        [Theory]
        [InlineData(0f, 0f, 0f, 0f, 0f)]
        public void TestValueCalculation(float a0, float a1, float a2, float x, float expected)
        {
            QuadraticFunction q = new QuadraticFunction(a0, a1, a2);
            Assert.NotNull(q);
            Assert.True(Near(expected, q.GetValueAt(x)));
            Assert.True(Near(expected, q.GetAt(x, 0)));
        }
    }
}
