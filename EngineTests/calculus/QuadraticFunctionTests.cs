using System;
using System.Collections.Generic;
using System.Linq;
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
		/// Tests quadratic function evaluation in cases where the expected value is a number. This test passes if the
		/// actual result is relatively near the expected result.
		/// </summary>
		/// <param name="a0">Constant term in quadratic function.</param>
		/// <param name="a1">Linear term in quadratic function.</param>
		/// <param name="a2">Quadratic term in quadratic function.</param>
		/// <param name="x">Value to test at.</param>
		/// <param name="expected"></param>
		[Theory]
		[InlineData(0, 0, 0, 0, 0)]
		[InlineData(1000, 0, 0, 0, 1000)]
		[InlineData(1000, 0, 0, 87429, 1000)]
		[InlineData(0, 1, 0, 0, 0)]
		[InlineData(0, 1, 0, 3.14159, 3.14159)]
		[InlineData(0, 1e10, 0, 1e5, 1e15)]
		[InlineData(42, 20, 0, 10, 242)]
		public void TestValueCalculation(double a0, double a1, double a2, double x, double expected)
		{
			QuadraticFunction q = new QuadraticFunction(a0, a1, a2);
			Assert.NotNull(q);
			Assert.Equal(expected, q.GetValueAt(x), 6);
			Assert.Equal(expected, q.GetNthDerivativeAt(x, 0), 6);
		}

		/// <summary>
		/// Tests evaluation of a function at derivatives from 0 to 4. Does not support expected
		/// NaN results. The 3rd and 4th derivatives of a quadratic function are always 0, so
		/// there is no parameter for this.
		/// </summary>
		/// <param name="a0">Constant term in the quadratic function.</param>
		/// <param name="a1">Linear term in the quadratic function.</param>
		/// <param name="a2">Quadratic term in the quadratic function.</param>
		/// <param name="x">Value of Location to test at.</param>
		/// <param name="atD0">Expected value of the function (the 0th derivative) at x.</param>
		/// <param name="atD1">Expected value of the first derivative at x.</param>
		/// <param name="atD2">Expected value of the second derivative at x.</param>
		[Theory]
		[InlineData(0, 0, 0, 0, 0, 0, 0)]
		[InlineData(1, 0, 0, 20, 1, 0, 0)]
		[InlineData(1, 2, 0, 10, 21, 2, 0)]
		[InlineData(1, 2, 4, 2, 21, 18, 8)]
		public void TestDerivativesAt(double a0, double a1, double a2, double x, double atD0, double atD1, double atD2)
		{
			QuadraticFunction q = new QuadraticFunction(a0, a1, a2);
			Assert.NotNull(q);
			Assert.Equal(q.GetNthDerivativeAt(x, 0), atD0, 6);
			Assert.Equal(q.GetNthDerivativeAt(x, 1), atD1, 6);
			Assert.Equal(q.GetDerivativeAt(x), atD1, 6);
			Assert.Equal(q.GetNthDerivativeAt(x, 2), atD2, 6);
			Assert.Equal(q.GetNthDerivativeAt(x, 3), 0);
			Assert.Equal(q.GetNthDerivativeAt(x, 4), 0);
		}


		[Theory]
		[InlineData(0, 1, 1, new double[] { -1, 0 })]
		public void TestSolve(double a0, double a1, double a2, double[] want)
		{
			IList<double> got = QuadraticFunction.Solve(a0, a1, a2).ToList();
			Assert.Equal(want.Length, got.Count);
			// There are no guarantees as to the order of values returned, so sort both sequences and compare items
			// at equal locations in the sorted sequence. Expect them all to match. We don't need to try any other
			// permutations of these lists to find out if there's some way they do match:
			//
			// If the smallest unmatched elements of each set are not adequately near each other, there is no way to
			// match the smaller of the two to any element in its other set. We have just compared it against the
			// smallest element in the other set, and found it to be too large. It cannot be too small, because we
			// are specifically considering the smaller element of the mismatched pair. Since the too-large element
			// was a smallest element of its set, all other elements are at least that large. Therefore, there is no
			// element small enough to match the smallest unmatched element between both sets, so the sets are unequal.
			// Backtracking does not help, because to find a smaller element of the opposite set, we must take it away
			// from its match to an element that is no larger than the one we are now matching (because all larger
			// elements are still in the unmatched portion of the set), and now _that_ element has the same problem
			// but worse.
			//
			// Therefore, there is no reason to attempt any other order than an element-by-element pairwise comparison
			// of the sorted sequences. ∎
			Array.Sort(want);
			int i = 0;
			var gotSorted = from c in got orderby c select c;
			foreach (double f in gotSorted)
			{
				Assert.Equal(want[i++], f, 6);
			}
		}
	}
}
