using Godot;
using GlmSharp;

using FreedomOfFormFoundation.AnatomyEngine.Renderable;

namespace FreedomOfFormFoundation.AnatomyRenderer
{
	public class GodotMeshConverter : ArrayMesh
	{
		// Convert an AnatomyEngine mesh to a Godot mesh:
		public GodotMeshConverter(UVMesh mesh)
		{
			// Initialize the arrays that are needed:
			Godot.Collections.Array arrays = new Godot.Collections.Array();
			arrays.Resize((int)Mesh.ArrayType.Max);
			
			int vertexCount = mesh.VertexList.Count;
			
			Vector3[] normalArray = new Vector3[vertexCount];
			Vector3[] vertexArray = new Vector3[vertexCount];

			// Populate the arrays from the input mesh data:
			for (int i = 0; i < vertexCount; i++)
			{
				vec3 vertex = mesh.VertexList[i].Position;
				vec3 normal = mesh.VertexList[i].Normal;
				vertexArray[i] = new Vector3(vertex.x, vertex.y, vertex.z);
				normalArray[i] = new Vector3(normal.x, normal.y, normal.z);
			}

			// The index list doesn't need to be converted:
			int[] indexArray = mesh.IndexList.ToArray();

			// Put the data arrays in a larger array for Godot to understand what the arrays represent:
			arrays[(int)Mesh.ArrayType.Vertex] = vertexArray;
			arrays[(int)Mesh.ArrayType.Normal] = normalArray;
			arrays[(int)Mesh.ArrayType.Index] = indexArray;
			
			// Finally, upload the mesh to GPU:
			this.AddSurfaceFromArrays(PrimitiveType.Triangles, arrays);
		}
	}
}
