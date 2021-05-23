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
using Xunit;
using FreedomOfFormFoundation.AnatomyEngine;

// IDE-specific warning suppression comments go here.
// ReSharper warns about "obvious equals", when I have the same literal expression on each side of the == operator.
// That's often useful because it's usually a mistake, but here it's a very deliberate part of testing == itself.
// ReSharper disable EqualExpressionComparison

namespace EngineTests
{
    public class Tests
    {
        [Fact]
        public void TestAddition()
        {
            Assert.Equal(new Real(2) + new Real(2), new Real(4));
            Assert.Equal(new Real(2) + new Real(-2), new Real(0));
            Assert.Equal(new Real(-1000000.0) + Real.PositiveInfinity, Real.PositiveInfinity);
            Assert.Equal(Real.PositiveInfinity + Real.NegativeInfinity, Real.NaN);
            Assert.Equal(new Real(9999999) + Real.NegativeInfinity, Real.NegativeInfinity);
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
    }
}