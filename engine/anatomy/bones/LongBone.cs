using System.Collections.Generic;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Anatomy.Bones
{
	public class LongBone : Bone
	{
		ContinuousMap<Vector2, float> Radius { get; set; }
		Curve CenterCurve { get; set; }
		
		public LongBone(Curve centerCurve, ContinuousMap<Vector2, float> radius)
		{
			this.CenterCurve = centerCurve;
			this.Radius = radius;
		}
		
		public LongBone(Curve centerCurve, ContinuousMap<float, float> radius)
			: this(centerCurve, new DomainToVector2<float>(new Vector2(0.0f, 1.0f), radius))
		{
			
		}
		
		public LongBone(Curve centerCurve, float radius)
			: this(centerCurve, new ConstantFunction<Vector2, float>(radius))
		{
			
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
