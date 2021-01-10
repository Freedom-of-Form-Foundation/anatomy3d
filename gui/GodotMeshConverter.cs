using Godot;
using System;
using System.Collections.Generic;
using Numerics = System.Numerics;

using FreedomOfFormFoundation.AnatomyEngine.Geometry;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;
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
			
			Godot.Vector3[] normal_array = new Godot.Vector3[vertexCount];
			Godot.Vector3[] vertex_array = new Godot.Vector3[vertexCount];
			
			// Populate the arrays from the input mesh data:
			for (int i = 0; i < vertexCount; i++)
			{
				Numerics.Vector3 vertex = mesh.VertexList[i].Position;
				Numerics.Vector3 normal = mesh.VertexList[i].Normal;
				vertex_array[i] = new Godot.Vector3(vertex.X, vertex.Y, vertex.Z);
				normal_array[i] = new Godot.Vector3(normal.X, normal.Y, normal.Z);
			}
			
			// The index list doesn't need to be converted:
			int[] index_array = mesh.IndexList.ToArray();
			
			// Put the data arrays in a larger array for Godot to understand what the arrays represent:
			arrays[(int)Mesh.ArrayType.Vertex] = vertex_array;
			arrays[(int)Mesh.ArrayType.Normal] = normal_array;
			arrays[(int)Mesh.ArrayType.Index] = index_array;
			
			// Finally, upload the mesh to GPU:
			this.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
		}
	}
}
