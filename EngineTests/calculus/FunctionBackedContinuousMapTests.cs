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
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace EngineTests
{
    public class FunctionBackedContinuousMapTests
    {
        [Fact]
        public void TestBasicFunctionality()
        {
            var c = new FunctionBackedContinuousMap<int, int>(x => x + 1);
            Assert.NotNull(c);
            Assert.Equal(5, c.GetValueAt(4));
            Assert.Equal(3, c.F(2));
            Assert.IsAssignableFrom<ContinuousMap<int, int>>(c);
        }

        // Helper function for testing the implicit FunctionBackedContinuousMap constructor.
        private static int TimesTwo(int x) => x * 2;

        [Fact]
        public void TestCasts()
        {
            // C# can't go straight from the function to a ContinuousMap, because technically it's already a cast to
            // become a Func (or any delegate type), double implicit casts are not considered when looking for
            // conversion operators, and there's no way to specify "method set" as the source for an implicit cast.
            // Oh well. Still, if we explicitly cast to Func, our conversion works:
            ContinuousMap<int, int> c = (Func<int, int>) TimesTwo;
            Assert.NotNull(c);
            Assert.Equal(10, c.GetValueAt(5));
            Assert.IsAssignableFrom<FunctionBackedContinuousMap<int, int>>(c);
            Assert.Equal(4, ((FunctionBackedContinuousMap<int, int>) c).F(2));

            FunctionBackedContinuousMap<int, int> d = (Func<int, int>) TimesTwo;
            Assert.NotNull(d);
            Assert.Equal(10, d.GetValueAt(5));
            Assert.Equal(4, d.F(2));
            Assert.IsAssignableFrom<ContinuousMap<int, int>>(d);

            var e = (ContinuousMap<int, int>) TimesTwo;
            Assert.NotNull(e);
            Assert.Equal(10, e.GetValueAt(5));
            Assert.IsAssignableFrom<FunctionBackedContinuousMap<int, int>>(e);
            Assert.Equal(4, ((FunctionBackedContinuousMap<int, int>) e).F(2));
        }
    }
}
