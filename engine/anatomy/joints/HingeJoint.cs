using System.Collections;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Anatomy.Joints
{
    public class HingeJoint : Joint
    {
        Cylinder articularSurface;
        
        public HingeJoint(Line centerLine, ContinuousMap<float, float> radius)
        {
            articularSurface = new Cylinder(centerLine, radius);
        }
        
        public HingeJoint(Line centerLine, float radius)
        {
            articularSurface = new Cylinder(centerLine, radius);
        }
        
        /// <summary>
        ///     Returns the surface geometry used by this Hinge Joint.
        /// </summary>
        public override Surface GetArticularSurface()
        {
            return articularSurface;
        }
    }
}
