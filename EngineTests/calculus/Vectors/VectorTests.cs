
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
            Assert.Equal(new Vector<ZeroSpace>(z, new Real[]{}),
                new Vector<ZeroSpace>(z, new Real[]{}));
            Assert.Equal(new Vector<ZeroSpace>(z, new Real[]{}),
                new Vector<ZeroSpace>(new ZeroSpace(), new Real[]{}));

            WorldSpace w = new WorldSpace();
            Assert.Equal(new Vector<WorldSpace>(w, 1, 1, 1),
                new Vector<WorldSpace>(w, 1, 1, 1));
            Assert.NotEqual(new Vector<WorldSpace>(w, 1, 1, 1),
                new Vector<WorldSpace>(w, 1, 1, 2));
        }
    }
}
