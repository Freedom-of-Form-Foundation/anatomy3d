﻿/*
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

// IDE-specific warning suppression comments go here.
// ReSharper warns about "obvious equals", when I have the same literal expression on each side of the == operator.
// That's often useful because it's usually a mistake, but here it's a very deliberate part of testing == itself.
// ReSharper disable EqualExpressionComparison

// Specify REALTYPE_DOUBLE, REALTYPE_FLOAT, or REALTYPE_DECIMAL in your compiler flags to set
// behavior of Real. Otherwise, defaults are DOUBLE for a production build and FLOAT for a
// debug build, to reveal numerical instability. This logic is also in Real.cs, but it's a
// separate assembly so its flags don't apply here.
#if !REALTYPE_DOUBLE && !REALTYPE_FLOAT && !REALTYPE_DECIMAL
#if DEBUG
#define REALTYPE_FLOAT
#else
#define REALTYPE_DOUBLE
#endif
#endif

using System;
using Xunit;
using FreedomOfFormFoundation.AnatomyEngine;

namespace EngineTests
{
    public class RealTests
    {
        [Fact]
        public void TestAddition()
        {
            Assert.Equal(new Real(4), new Real(2) + new Real(2));
            Assert.Equal(new Real(0), new Real(2) + new Real(-2));
            Assert.Equal(Real.PositiveInfinity, new Real(-1000000.0) + Real.PositiveInfinity);
            Assert.Equal(Real.NaN, Real.PositiveInfinity + Real.NegativeInfinity);
            Assert.Equal(Real.NegativeInfinity, new Real(9999999) + Real.NegativeInfinity);
            Assert.NotEqual(new Real(5), new Real(2) + new Real(2.0));
        }
        
        [Fact]
        public void TestEqualsFunction()
        {
            Assert.True(new Real(0).Equals(new Real(0)));
            Assert.False(new Real(100).Equals(new Real(150)));
            Assert.True(new Real(500).Equals(new Real(500.0)));
            Assert.True(Real.PositiveInfinity.Equals(Real.PositiveInfinity));
            Assert.True(Real.NegativeInfinity.Equals(Real.NegativeInfinity));
            Assert.False(Real.PositiveInfinity.Equals(Real.NegativeInfinity));
            // Although NaN compares as unequal to itself when using the == operator, the .Equals method <i>does</i>
            // detect NaNs as equal to themselves. Anything else would break the IEquatable/IEquatable<T> contract
            // and also break using this as a hash table key.
            Assert.True(Real.NaN.Equals(Real.NaN));
        }
        
        [Fact]
        public void TestEqualsOperator()
        {
            Assert.True(new Real(0) == new Real(0));
            Assert.False(new Real(100) == new Real(150));
            Assert.True(new Real(500) == new Real(500.0));
            Assert.True(Real.PositiveInfinity == Real.PositiveInfinity);
            Assert.True(Real.NegativeInfinity == Real.NegativeInfinity);
            Assert.False(Real.PositiveInfinity == Real.NegativeInfinity);
            
            // When using the == operator, NaN is unequal to itself, as specified in the IEEE 754 standard for
            // floating-point arithmetic.
            Assert.False(Real.NaN == Real.NaN);
        }
        // Denormalized values are different depending on the backing type of Real.
#if REALTYPE_DECIMAL
        [Theory]
        [InlineData(0d, false)]
        [InlineData(100d, true)]
        [InlineData(0.00002d, true)]
        [InlineData(4.5e-75d, true)]
        public void TestIsNormalSubnormalDecimal(Decimal x, bool wantNormal)
        {
            Real r = new Real(x);
            Assert.Equal(wantNormal, r.IsNormal);
            Assert.False(r.IsSubnormal);
        }
#elif REALTYPE_DOUBLE
        [Theory]
        [InlineData(0.0, false, false)]
        [InlineData(Double.NaN, false, false)]
        [InlineData(Double.NegativeInfinity, false, false)]
        [InlineData(Double.PositiveInfinity, false, false)]
        [InlineData(-0.0, false, false)]
        [InlineData(1.5, true, false)]
        [InlineData(4875293824932, true, false)]
        [InlineData(0.89812784, true, false)]
        [InlineData(5.5e-310, false, true)]
        public void TestIsNormalSubnormalDouble(double x, bool wantNormal, bool wantSubnormal)
        {
            Real r = new Real(x);
            Assert.Equal(wantNormal, r.IsNormal);
            Assert.Equal(wantSubnormal, r.IsSubnormal);
        }

#elif REALTYPE_FLOAT
        [Theory]
        [InlineData(0f, false, false)]
        [InlineData(Single.NaN, false, false)]
        [InlineData(Single.NegativeInfinity, false, false)]
        [InlineData(Single.PositiveInfinity, false, false)]
        [InlineData(-0f, false, false)]
        [InlineData(1f, true, false)]
        [InlineData(19384.29f, true, false)]
        [InlineData(2.9387e-39f, false, true)]
        [InlineData(4.78e-42f, false, true)]
        public void TestIsNormalSubnormalFloat(float x, bool wantNormal, bool wantSubnormal)
        {
            Real r = new Real(x);
            Assert.Equal(wantNormal, r.IsNormal);
            Assert.Equal(wantSubnormal, r.IsSubnormal);
        }
#else
    #error Unknown backing type for Real - cannot test IsNormal, IsSubnormal.
#endif
    }
}
