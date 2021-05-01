using System.Collections.Generic;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public abstract class Curve : ContinuousMap<float, Vector3>
	{
		/// <summary>
		///     Returns the position in 3D space of the curve based on the parametric variable <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The parameter along the length of the curve. <paramref name="t"/> must be in the range
		/// \f$[ 0, 1 ]\f$.</param>
		public abstract Vector3 GetPositionAt(float t);
		
		/// <summary>
		///     Returns the tangent vector of the curve based on the parametric variable <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The parameter along the length of the curve. <paramref name="t"/> must be in the range
		/// \f$[ 0, 1 ]\f$.</param>
		public abstract Vector3 GetTangentAt(float t);
		
		/// <summary>
		///     Returns the normal vector of the curve based on the parametric variable <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The parameter along the length of the curve. <paramref name="t"/> must be in the range
		/// \f$[ 0, 1 ]\f$.</param>
		public abstract Vector3 GetNormalAt(float t);
		
		/// <summary>
		///     Returns the position in 3D space of the start point of the curve.
		///     Should return the same value as <c>getPositionAt(0.0)</c>.
		/// </summary>
		public virtual Vector3 GetStartPosition()
		{
			return GetPositionAt(0.0f);
		}
		
		/// <summary>
		///     Returns the position in 3D space of the end point of the curve.
		///     Should return the same value as <c>getPositionAt(1.0)</c>.
		/// </summary>
		public virtual Vector3 GetEndPosition()
		{
			return GetPositionAt(1.0f);
		}
		
		public override Vector3 GetValueAt(float t)
		{
			return GetPositionAt(t);
		}
	}
}
