using System.Collections;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Anatomy.Bones
{
    public class LongBone : Bone
    {
        /// <summary>
        ///     Returns the surface geometry of the bone.
        /// </summary>
        public override Surface GetGeometry()
        {
            Line centerLine = new Line(new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
            return new Capsule(new Cylinder(centerLine, 0.5f));
        }
    }
}
