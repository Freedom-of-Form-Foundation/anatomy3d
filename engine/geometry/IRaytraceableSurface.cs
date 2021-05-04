using System.Collections.Generic;
using System.Numerics;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;
using FreedomOfFormFoundation.AnatomyEngine.Renderable;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	/// <summary>
	///     An interface defining the ability to raytrace a surface analytically.
	/// </summary>
	public interface IRaytraceableSurface
	{
		/// <summary>
		///		Intersects the surface with a ray \f$\vec{p} + t \vec{s}\f$, returning all \f$t\f$ for which there
		///		is an intersection. \f$\vec{p}\f$ is given by <c>rayStart</c>, and \f$\vec{s}\f$ is given by
		///		<c>rayDirection</c>.
		/// </summary>
		/// <param name="rayStart">
		///		The point from which the ray is cast.
		/// </param>
		/// <param name="rayDirection">
		///		The direction in which the ray is cast.
		/// </param>
		float RayIntersect(Vector3 rayStart, Vector3 rayDirection);
	}
}
