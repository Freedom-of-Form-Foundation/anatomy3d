using System.Collections;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Anatomy
{
	public abstract class Joint : BodyPart
	{
		/// <summary>
		///     Returns the articular surface of this joint.
		/// </summary>
		public abstract Surface GetArticularSurface();
		
		/// <summary>
		///     Returns the raytraceable surface geometry used by this Hinge Joint.
		/// </summary>
		public abstract IExtendedRaytraceableSurface GetExtendedRaytraceableSurface();

		
		/// <summary>
		///     Returns the surface geometry used by this BodyPart.
		/// </summary>
		public override Surface GetGeometry()
		{
			return this.GetArticularSurface();
		}
	}
}
