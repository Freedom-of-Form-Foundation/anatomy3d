using System.Collections.Generic;
using System.Numerics;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
    public abstract class ContinuousMap<I, O>
    {
        public abstract O GetValueAt(I t);
        
    }
}
