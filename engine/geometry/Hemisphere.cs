using System.Collections.Generic;
using System.Numerics;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public class Hemisphere : Surface
	{
		public Hemisphere(ContinuousMap<Vector2, float> radius, Vector3 center, Vector3 direction)
		{
			this.Radius = radius;
			this.Center = center;
			this.Direction = direction;
		}
		
		ContinuousMap<Vector2, float> Radius { get; set; }
		
		Vector3 Center { get; set; }
		
		private Vector3 direction;
		Vector3 Direction
		{
			get { return direction; }
			set { direction = Vector3.Normalize(value); }
		}
		
		public static int CalculateVertexCount(int resolutionU, int resolutionV)
		{
			return resolutionU * resolutionV + 1;
		}
		
		public static int CalculateIndexCount(int resolutionU, int resolutionV)
		{
			return resolutionU * resolutionV * 6 + resolutionU * 3;
		}
		
		public Vector3 GetNormalAt(Vector2 uv)
		{
			float u = uv.X;
			float v = uv.Y;
			
			Vector3 up = new Vector3(0.0f, 0.0f, 1.0f);
			
			Vector3 pointTangent = this.direction;
			Vector3 pointNormal = Vector3.Normalize(Vector3.Cross(pointTangent, up));
			Vector3 pointBinormal = Vector3.Normalize(Vector3.Cross(pointTangent, pointNormal));
			
			// Calculate the position of the rings of vertices:
			float x = (float)Math.Sin(v) * (float)Math.Cos(u);
			float y = (float)Math.Sin(v) * (float)Math.Sin(u);
			float z = (float)Math.Cos(v);
			
			return x*pointNormal + y*pointBinormal + z*pointTangent;
		}
		
		public Vector3 GetPositionAt(Vector2 uv)
		{
			Vector3 translation = Center;
			float radius = this.Radius.GetValueAt(uv);
			
			return translation + radius*GetNormalAt(uv);
		}
		
		public override List<Vertex> GenerateVertexList(int resolutionU, int resolutionV)
		{
			List<Vertex> output = new List<Vertex>(CalculateVertexCount(resolutionU, resolutionV));
			
			// Load all required variables:
			Vector3 up = new Vector3(0.0f, 0.0f, 1.0f);
			
			Vector3 pointTangent = this.direction;
			Vector3 pointNormal = Vector3.Normalize(Vector3.Cross(pointTangent, up));
			Vector3 pointBinormal = Vector3.Normalize(Vector3.Cross(pointTangent, pointNormal));
			
			Vector3 translation = Center;
			
			// Get the radius at the top of the hemisphere:
			float topRadius = this.Radius.GetValueAt(new Vector2(0.0f, 0.5f*(float)Math.PI));

			// Generate the first point at the pole of the hemisphere:
			output.Add(new Vertex(translation + topRadius*pointTangent, pointTangent));
			
			// Generate rings of the other points:
			for (int j = 1; j < (resolutionV+1); j++)
			{
				for (int i = 0; i < resolutionU; i++)
				{
					// First find the normalized uv-coordinates, u = [0, 2pi], v = [0, 1/2pi]:
					float u = (float)i/(float)resolutionU * 2.0f * (float)Math.PI;
					float v = (float)j/(float)resolutionV * 0.5f * (float)Math.PI;
					
					// Calculate the position of the rings of vertices:
					float x = (float)Math.Sin(v) * (float)Math.Cos(u);
					float y = (float)Math.Sin(v) * (float)Math.Sin(u);
					float z = (float)Math.Cos(v);
					
					float radius = this.Radius.GetValueAt(new Vector2(u, v));
					
					Vector3 surfaceNormal = x*pointNormal + y*pointBinormal + z*pointTangent;
					Vector3 surfacePosition = translation + radius*surfaceNormal;
					
					output.Add(new Vertex(surfacePosition, surfaceNormal));
				}
			}
			
			return output;
		}
		
		public override List<int> GenerateIndexList(int resolutionU, int resolutionV, int indexOffset = 0)
		{
			List<int> output = new List<int>(CalculateIndexCount(resolutionU, resolutionV));
			
			// Add a triangle fan on the pole of the hemisphere:
			for (int i = 0; i < resolutionU-1; i++)
			{
				output.Add(indexOffset + 1 + i);
				output.Add(indexOffset + 0);
				output.Add(indexOffset + 2 + i);
			}
			
			// Stitch final triangle on the pole of the hemisphere:
			output.Add(indexOffset + resolutionU);
			output.Add(indexOffset + 0);
			output.Add(indexOffset + 1);
			
			// Add the remaining rings:
			for (int j = 0; j < resolutionV - 1; j++)
			{
				// Add a ring of triangles:
				for (int i = 0; i < resolutionU-1; i++)
				{
					output.Add(indexOffset + 1 + i + resolutionU*(j+1));
					output.Add(indexOffset + 1 + i + resolutionU*j);
					output.Add(indexOffset + 1 + (i+1) + resolutionU*j);

					output.Add(indexOffset + 1 + (i+1) + resolutionU*(j+1));
					output.Add(indexOffset + 1 + i + resolutionU*(j+1));
					output.Add(indexOffset + 1 + (i+1) + resolutionU*j);
				}
				
				// Stitch the end of the ring of triangles:
				output.Add(indexOffset + 1 + resolutionU-1 + resolutionU*(j+1));
				output.Add(indexOffset + 1 + resolutionU-1 + resolutionU*j);
				output.Add(indexOffset + 1 + resolutionU*j);

				output.Add(indexOffset + 1 + resolutionU*(j+1));
				output.Add(indexOffset + 1 + resolutionU-1 + resolutionU*(j+1));
				output.Add(indexOffset + 1 + resolutionU*j);
			}

			return output;
		}
	}
}
