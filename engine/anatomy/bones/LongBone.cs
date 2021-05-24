using System.Collections.Generic;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Anatomy;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Anatomy.Bones
{
	public class LongBone : Bone
	{
		public ContinuousMap<Vector2, float> Radius { get; set; }
		public Curve CenterCurve { get; set; }
		
		public List<(Joint joint, RayCastDirection direction)> InteractingJoints { get; set; }
		
		public LongBone(Curve centerCurve, ContinuousMap<Vector2, float> radius)
		{
			this.CenterCurve = centerCurve;
			this.Radius = radius;
			this.InteractingJoints = new List<(Joint joint, RayCastDirection direction)>(0);
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
			ContinuousMap<Vector2, float> deformations = Radius;
			foreach (var i in InteractingJoints)
			{
				MoldCastMap boneHeightMap = new MoldCastMap(CenterCurve, i.joint.GetRaytraceableSurface(), deformations, i.direction);
				deformations = boneHeightMap;
			}
			
			return new Capsule(CenterCurve, deformations);
		}
	}
}
