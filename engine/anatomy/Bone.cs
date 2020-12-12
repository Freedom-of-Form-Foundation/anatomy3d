using System.Collections;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Anatomy
{
    public abstract class Bone : BodyPart
    {
        
        
        /// <summary>
        ///     Returns the surface geometry used by this Bone.
        /// </summary>
        public override Surface GetGeometry()
        {
            return new Hemisphere(1.0f, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
        }
    }
}
