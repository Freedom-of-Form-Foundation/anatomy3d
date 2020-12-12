using System.Collections.Generic;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Renderable
{
    public class Mesh
    {
        /// <summary>
        ///     The list containing 3D positions and auxiliary data of each vertex in the mesh. Relates to VBO.
        /// </summary>
        public List<Vertex> vertexList;
        
        /// <summary>
        ///     The list containing the indices of vertices, where each group of three indices corresponds to a single
        ///     triangle. Relates to IBO.
        /// </summary>
        public List<int> indexList;
    }
}
