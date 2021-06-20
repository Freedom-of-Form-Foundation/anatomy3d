using System.Collections;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Anatomy.Joints
{
	public class HingeJoint : Joint
	{
		SymmetricCylinder articularSurface;
		
		public HingeJoint(LineSegment centerLine, RaytraceableFunction1D radius)
		{
			articularSurface = new SymmetricCylinder(centerLine, radius);
		}
		
		public HingeJoint(LineSegment centerLine, RaytraceableFunction1D radius, ContinuousMap<float, float> startAngle, ContinuousMap<float, float> endAngle)
		{
			articularSurface = new SymmetricCylinder(centerLine, radius, startAngle, endAngle);
		}
		
		//public HingeJoint(LineSegment centerLine, float radius)
		//{
		//	articularSurface = new SymmetricCylinder(centerLine, radius);
		//}
		
		/// <summary>
		///     Returns the surface geometry used by this Hinge Joint.
		/// </summary>
		public override Surface GetArticularSurface()
		{
			return articularSurface;
		}
		
		/// <summary>
		///     Returns the raytraceable surface geometry used by this Hinge Joint.
		/// </summary>
		public override IRaytraceableSurface GetRaytraceableSurface()
		{
			return articularSurface;
		}
	}
}
