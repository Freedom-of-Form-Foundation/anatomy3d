using FreedomOfFormFoundation.AnatomyEngine.Calculus.Vectors;

namespace EngineTests.calculus.Vectors
{
    /// <summary>
    /// ArbitraryVectorSpace is a vector space with a name and dimension, which is the same as
    /// any other ArbitraryVectorSpace with the same name and dimension. It is not intended
    /// for production use - it is for test use only. It lets us create arbitrary type-equal
    /// and value-unequal vector spaces.
    /// </summary>
    public struct ArbitraryVectorSpace : IVectorSpace
    {
        public int Dim { get; }
        public string Name { get; }

        public ArbitraryVectorSpace(string name, int dim)
        {
            Name = name;
            Dim = dim;
        }
        public bool Equals(IVectorSpace other) =>
            other is ArbitraryVectorSpace a && Dim.Equals(a.Dim) && Name.Equals(a.Name);

        public override bool Equals(object obj) => obj is ArbitraryVectorSpace other && Equals(other);

        public override int GetHashCode() => Dim ^ Name.GetHashCode();

        public int Dimension() => Dim;
    }
}
