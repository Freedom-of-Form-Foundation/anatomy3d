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
    }
}