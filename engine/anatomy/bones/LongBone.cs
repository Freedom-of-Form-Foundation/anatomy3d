using System.Collections.Generic;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Anatomy.Bones
{
	public class LongBone : Bone
	{
		ContinuousMap<float, float> Radius { get; set; }
		Curve CenterCurve { get; set; }
		
		public LongBone(Curve centerCurve, ContinuousMap<float, float> radius)
		{
			this.CenterCurve = centerCurve;
			this.Radius = radius;
		}
		
		/// <summary>
		///     Returns the surface geometry of the bone.
		/// </summary>
		public override Surface GetGeometry()
		{
			return new Capsule(this.CenterCurve, this.Radius);
		}
	}
}
