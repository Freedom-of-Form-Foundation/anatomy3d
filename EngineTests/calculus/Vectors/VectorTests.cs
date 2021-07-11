
using System;
using FreedomOfFormFoundation.AnatomyEngine;
using Xunit;
using FreedomOfFormFoundation.AnatomyEngine.Calculus.Vectors;

namespace EngineTests.calculus.Vectors
{
    public class VectorTests
    {
        [Fact]
        public void TestEquality()
        {
            ZeroSpace z = new ZeroSpace();
            Assert.Equal(new Vector<ZeroSpace>(z),
                new Vector<ZeroSpace>(z));
            Assert.Equal(new Vector<ZeroSpace>(z),
                new Vector<ZeroSpace>(new ZeroSpace()));

            WorldSpace w = new WorldSpace();
            Assert.Equal(new Vector<WorldSpace>(w, 1, 1, 1),
                new Vector<WorldSpace>(w, 1, 1, 1));
            Assert.NotEqual(new Vector<WorldSpace>(w, 1, 1, 1),
                new Vector<WorldSpace>(w, 1, 1, 2));

            ArbitraryVectorSpace arbA = new ArbitraryVectorSpace("a", 2);
            ArbitraryVectorSpace arbB = new ArbitraryVectorSpace("b", 2);

            Assert.Equal(new Vector<ArbitraryVectorSpace>(arbA, 5, 10),
                new Vector<ArbitraryVectorSpace>(arbA, new double[]{5, 10}));
            Assert.NotEqual(new Vector<ArbitraryVectorSpace>(arbA, 5, 10),
                new Vector<ArbitraryVectorSpace>(arbB, 5, 10));
        }

        [Fact]
        public void TestIndexer()
        {
            Vector<WorldSpace> v = new Vector<WorldSpace>(new WorldSpace(), 0, 1, 2);
            Assert.Equal(0.0, v[0]);
            Assert.Equal(1.0, v[1]);
            Assert.Equal(2.0, v[2]);
            Assert.Throws(typeof(IndexOutOfRangeException), () => v[3]);
        }

        [Fact]
        public void TestDim()
        {
            Assert.Equal(3, new Vector<WorldSpace>(new WorldSpace(), 1, 2, 3).Dim);
            Assert.Equal(0, new Vector<ZeroSpace>(new ZeroSpace()).Dim);
        }

    }
}
