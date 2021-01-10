using System.Collections.Generic;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Renderable;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public abstract class Surface
	{
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
