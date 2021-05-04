using System.Collections.Generic;
using System.Numerics;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;
using FreedomOfFormFoundation.AnatomyEngine.Renderable;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public abstract class Surface
	{
		/// <summary>
		///     Returns the surface normal at the uv coordinate given.
		/// </summary>
		public abstract Vector3 GetNormalAt(Vector2 uv);
		
		/// <summary>
		///     Returns the surface position at the uv coordinate given.
		/// </summary>
		public abstract Vector3 GetPositionAt(Vector2 uv);
		
		/// <summary>
		///     Intersects the surface with a ray \f$\vec{p} + t \vec{s}\f$, returning all \f$t\f$ for which there
		///		is an intersection.
		/// </summary>
		public virtual float RayIntersect(Vector3 rayStart, Vector3 rayDirection)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		///     Generates the vertex positions of this uv-parametrized surface. The returned list will contain
		///     approximately <paramref name="resolutionU"/> times <paramref name="resolutionV"/> vertices in total. The
		///     meaning of the coordinates u and v depends on the specific type of surface.
		/// </summary>
		public abstract List<Vertex> GenerateVertexList(int resolutionU, int resolutionV);
		
		/// <summary>
		///     Generates the indices pointing to vertices of this uv-parametrized surface. The returned list can
		///     contain any number of indices, since there will be a variable number of triangles depending on the
		///     surface type.
		/// </summary>
		public abstract List<int> GenerateIndexList(int resolutionU, int resolutionV, int indexOffset = 0);
		
		/// <summary>
		///     Generates the vertex positions and indices of this uv-parametrized surface. The returned list will
		///     contain approximately <paramref name="resolutionU"/> times <paramref name="resolutionV"/> vertices in
		///     total. The meaning of the coordinates u and v depends on the specific type of surface.
		/// </summary>
		public virtual UVMesh GenerateMesh(int resolutionU, int resolutionV, int indexOffset = 0)
		{
			return new UVMesh(this, resolutionU, resolutionV, indexOffset);
		}
	}
}
