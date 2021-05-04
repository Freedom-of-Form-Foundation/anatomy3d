using System.Collections.Generic;
using System.Numerics;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	///     A MoldCastMap projects one surface onto a mold surface, giving the distance between the two surfaces
	///		on each coordinate point. A CurveMoldCastMap uses a curve to project its rays from, simplifying
	///		computation. This map can for example be used to shape the Cylinder and the Capsule surfaces.
	/// </summary>
	public class CurveMoldCastMap : ContinuousMap<Vector2, float>
	{
		private Surface raycastSurface;
		private IRaytraceableSurface moldSurface;
		
		public CurveMoldCastMap(Curve raycastCurve, IRaytraceableSurface moldSurface)
		{
			this.raycastSurface = new Capsule(raycastCurve, 0.0f);
			this.moldSurface = moldSurface;
		}
		
		public override float GetValueAt(Vector2 uv)
		{
			return moldSurface.RayIntersect(raycastSurface.GetPositionAt(uv), Vector3.Normalize(raycastSurface.GetNormalAt(uv)));
		}
	}
}
