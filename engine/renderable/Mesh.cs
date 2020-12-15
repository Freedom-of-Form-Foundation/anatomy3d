using System.Collections.Generic;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Renderable
{
    public class UVMesh
    {
        public UVMesh(Surface surface, int resolutionU, int resolutionV, int indexOffset = 0)
        {
            this.VertexList = surface.GenerateVertexList(resolutionU, resolutionV);
            this.IndexList = surface.GenerateIndexList(resolutionU, resolutionV, indexOffset);
            this.ResolutionU = resolutionU;
            this.ResolutionV = resolutionV;
        }
        
        public int ResolutionU { get; }
        public int ResolutionV { get; }
        
        /// <summary>
        ///     The list containing 3D positions and auxiliary data of each vertex in the mesh. Relates to VBO.
        /// </summary>
        public List<Vertex> VertexList { get; set; }
        
        /// <summary>
        ///     The list containing the indices of vertices, where each group of three indices corresponds to a single
        ///     triangle. Relates to IBO.
        /// </summary>
        public List<int> IndexList { get; set; }
    }
}
