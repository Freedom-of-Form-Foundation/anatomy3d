using System.Collections;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Anatomy.Joints
{
	public class HingeJoint : Joint
	{
		SymmetricCylinder articularSurface;
		
		public HingeJoint(Line centerLine, ContinuousMap<float, float> radius)
		{
			articularSurface = new SymmetricCylinder(centerLine, radius);
		}
		
		public HingeJoint(Line centerLine, ContinuousMap<float, float> radius, ContinuousMap<float, float> startAngle, ContinuousMap<float, float> endAngle)
		{
			articularSurface = new SymmetricCylinder(centerLine, radius, startAngle, endAngle);
		}
		
		public HingeJoint(Line centerLine, float radius)
		{
			articularSurface = new SymmetricCylinder(centerLine, radius);
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
