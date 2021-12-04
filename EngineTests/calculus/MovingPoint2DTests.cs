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

using System.Collections.Generic;
using FreedomOfFormFoundation.AnatomyEngine;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;
using Xunit;

namespace EngineTests.calculus
{
    public class MovingPoint2DTests
    {
        [Fact]
        public void TestReadWriteX()
        {
            int count = 0;

            void Counter()
            {
                count++;
            }

            MovingPoint2D p = new MovingPoint2D(Counter);
            Assert.Equal(0, count);
            Assert.Equal(0.0, p.X);
            p.X = 250;
            Assert.Equal(250.0, p.X);
            Assert.Equal(1, count);
            p.X = -1024.125;
            Assert.Equal(-1024.125, p.X);
            Assert.Equal(2, count);
        }

        [Fact]
        public void TestReadWriteY()
        {
            int count = 0;

            void Counter()
            {
                count++;
            }

            MovingPoint2D p = new MovingPoint2D(Counter);
            Assert.Equal(0, count);
            Assert.Equal(0, p.Y);
            p.Y = 250;
            Assert.Equal(250, p.Y);
            Assert.Equal(1, count);
            p.Y = -1024.125;
            Assert.Equal(-1024.125, p.Y);
            Assert.Equal(2, count);
        }

        [Fact]
        public void TestVaryingSort()
        {
            int countA = 0;
            MovingPoint2D pA = new MovingPoint2D(() => { countA++; }, 0, 0);

            int countB = 0;
            MovingPoint2D pB = new MovingPoint2D(() => { countB++; }, 1, 1);

            int countC = 0;
            MovingPoint2D pC = new MovingPoint2D(() => { countC++; }, 2, 2);

            List<MovingPoint2D> list = new List<MovingPoint2D> {pC, pB, pA};
            Assert.Equal(new List<MovingPoint2D>{pC, pB, pA}, list);
            list.Sort();
            Assert.Equal(new List<MovingPoint2D>{pA, pB, pC}, list);
            Assert.Equal(0, countA);
            Assert.Equal(0, countB);
            Assert.Equal(0, countC);

            pA.X = 3;
            list.Sort();
            Assert.Equal(new List<MovingPoint2D>{pB, pC, pA}, list);

            pB.X = 3;
            list.Sort();
            Assert.Equal(new List<MovingPoint2D>{pC, pA, pB}, list);

            pA.Y = 5;
            list.Sort();
            Assert.Equal(new List<MovingPoint2D>{pC, pB, pA}, list);
        }
    }
}
