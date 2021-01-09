using System.Collections.Generic;
using System.Numerics;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public class Cylinder : Surface
	{
		public Cylinder(Curve centerCurve, float radius)
		{
			this.CenterCurve = centerCurve;
			this.Radius = new ConstantFunction<float, float>(radius);
		}
		
		public Cylinder(Curve centerCurve, ContinuousMap<float, float> radius)
		{
			this.CenterCurve = centerCurve;
			this.Radius = radius;
		}
		
		public ContinuousMap<float, float> Radius { get; set; }
		public Curve CenterCurve { get; set; }
		
		public static int CalculateVertexCount(int resolutionU, int resolutionV)
		{
			return resolutionU * (resolutionV + 1);
		}
		
		public static int CalculateIndexCount(int resolutionU, int resolutionV)
		{
			return resolutionU * resolutionV * 6;
		}
		
		public override List<Vertex> GenerateVertexList(int resolutionU, int resolutionV)
		{
			List<Vertex> output = new List<Vertex>(CalculateVertexCount(resolutionU, resolutionV));
			
			for (int j = 0; j < (resolutionV + 1); j++)
			{
				float v = (float)j/(float)resolutionV;
				
				// Generate a rotation matrix to rotate each circle in the cylinder
				// to align with the tangent vector of the center curve. The matrix
				// rotates the 'up' vector onto the 'direction' vector, using
				// Rodrigues' Rotation Formula:
				Vector3 up = new Vector3(0.0f, 0.0f, 1.0f);
				
				// To prevent division by zero, flip the direction if facing the other way:
				float sign = 1.0f;
				if(Vector3.Dot(up, CenterCurve.GetTangentAt(v)) < 0.0f)
				{
					sign = -1.0f;
				}
				
				Vector3 direction = Vector3.Normalize(CenterCurve.GetTangentAt(v));
				Vector3 k = Vector3.Cross(sign*direction, up);
				
				Matrix4x4 identity = new Matrix4x4(1.0f, 0.0f, 0.0f, 0.0f,
												   0.0f, 1.0f, 0.0f, 0.0f,
												   0.0f, 0.0f, 1.0f, 0.0f,
												   0.0f, 0.0f, 0.0f, 1.0f);
				
				Matrix4x4 K = new Matrix4x4(0.0f, -k.Z,  k.Y, 0.0f,
											 k.Z, 0.0f, -k.X, 0.0f,
											-k.Y,  k.X, 0.0f, 0.0f,
											0.0f, 0.0f, 0.0f, 0.0f);
				
				Matrix4x4 rotationMatrix = identity + K + K*K*(1.0f/(1.0f + Vector3.Dot(up, sign*direction)));
				
				Matrix4x4 transformationMatrix = rotationMatrix;
				transformationMatrix.Translation = CenterCurve.GetPositionAt(v);
				
				float radius = Radius.GetValueAt(v);
				
				for (int i = 0; i < resolutionU; i++)
				{
					// First find the normalized uv-coordinates, u = [0, 2pi], v = [0, 1]:
					float u = (float)i/(float)resolutionU * 2.0f * (float)Math.PI;
					
					// Calculate the position of the rings of vertices:
					float x = (float)Math.Cos(u);
					float y = (float)Math.Sin(sign*u);
					float z = 0;
					
					Vector3 position = new Vector3(x, y, z);
					
					// Rotate the vector to orient the hemisphere correctly:
					Vector3 vertexPosition = Vector3.Transform(sign * radius * position, transformationMatrix);
					Vector3 normal = Vector3.Transform(sign*position, rotationMatrix);
					
					output.Add(new Vertex(vertexPosition, normal));
				}
			}
			
			return output;
		}
		
		public override List<int> GenerateIndexList(int resolutionU, int resolutionV, int indexOffset = 0)
		{
			List<int> output = new List<int>(CalculateIndexCount(resolutionU, resolutionV));
			
			// Add the remaining rings:
			for (int j = 0; j < resolutionV; j++)
			{
				// Add a ring of triangles:
				for (int i = 0; i < resolutionU-1; i++)
				{
					output.Add(indexOffset + i + resolutionV*j);
					output.Add(indexOffset + i + resolutionV*(j+1));
					output.Add(indexOffset + (i+1) + resolutionV*j);

					output.Add(indexOffset + i + resolutionV*(j+1));
					output.Add(indexOffset + (i+1) + resolutionV*(j+1));
					output.Add(indexOffset + (i+1) + resolutionV*j);
				}
				
				// Stitch the end of the triangles:
				output.Add(indexOffset + resolutionU-1 + resolutionV*j);
				output.Add(indexOffset + resolutionU-1 + resolutionV*(j+1));
				output.Add(indexOffset + resolutionV*j);

				output.Add(indexOffset + resolutionU-1 + resolutionV*(j+1));
				output.Add(indexOffset + resolutionV*(j+1));
				output.Add(indexOffset + resolutionV*j);
			}

			return output;
		}
	}
}
