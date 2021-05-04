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
			this.Radius = new ConstantFunction<Vector2, float>(radius);
			this.StartAngle = 0.0f;
			this.EndAngle = 2.0f * (float)Math.PI;
		}
		
		public Cylinder(Curve centerCurve, ContinuousMap<Vector2, float> radius)
		{
			this.CenterCurve = centerCurve;
			this.Radius = radius;
			this.StartAngle = 0.0f;
			this.EndAngle = 2.0f * (float)Math.PI;
		}
		
		public Cylinder(Curve centerCurve, ContinuousMap<Vector2, float> radius, ContinuousMap<float, float> startAngle, ContinuousMap<float, float> endAngle)
		{
			this.CenterCurve = centerCurve;
			this.Radius = radius;
			this.StartAngle = startAngle;
			this.EndAngle = endAngle;
		}
		
		public ContinuousMap<Vector2, float> Radius { get; set; }
		public Curve CenterCurve { get; set; }
		public ContinuousMap<float, float> StartAngle { get; set; }
		public ContinuousMap<float, float> EndAngle { get; set; }
		
		public static int CalculateVertexCount(int resolutionU, int resolutionV)
		{
			return resolutionU * (resolutionV + 1);
		}
		
		public static int CalculateIndexCount(int resolutionU, int resolutionV)
		{
			return resolutionU * resolutionV * 6;
		}
		
		public override Vector3 GetNormalAt(Vector2 uv)
		{
			float u = uv.X;
			float v = uv.Y;
			
			Vector3 curveTangent = Vector3.Normalize(CenterCurve.GetTangentAt(v));
			Vector3 curveNormal = Vector3.Normalize(CenterCurve.GetNormalAt(v));
			Vector3 curveBinormal = Vector3.Cross(curveTangent, curveNormal);
			
			float startAngle = StartAngle.GetValueAt(v);
			float endAngle = EndAngle.GetValueAt(v);
			
			return (float)Math.Cos(u)*curveNormal + (float)Math.Sin(u)*curveBinormal;
		}
		
		public override Vector3 GetPositionAt(Vector2 uv)
		{
			float v = uv.Y;
				
			Vector3 translation = CenterCurve.GetPositionAt(v);
			float radius = Radius.GetValueAt(uv);
			
			return translation + radius*GetNormalAt(uv);
		}
		
		public override List<Vertex> GenerateVertexList(int resolutionU, int resolutionV)
		{
			List<Vertex> output = new List<Vertex>(CalculateVertexCount(resolutionU, resolutionV));
			
			for (int j = 0; j < (resolutionV + 1); j++)
			{
				float v = (float)j/(float)resolutionV;
				
				// Find the values at each ring:
				Vector3 curveTangent = Vector3.Normalize(CenterCurve.GetTangentAt(v));
				Vector3 curveNormal = Vector3.Normalize(CenterCurve.GetNormalAt(v));
				Vector3 curveBinormal = Vector3.Cross(curveTangent, curveNormal);
				
				Vector3 translation = CenterCurve.GetPositionAt(v);
				
				float startAngle = StartAngle.GetValueAt(v);
				float endAngle = EndAngle.GetValueAt(v);
				
				for (int i = 0; i < resolutionU; i++)
				{
					// First find the normalized uv-coordinates, u = [0, 2pi], v = [0, 1]:
					float u = startAngle + (endAngle-startAngle)*(float)i/(float)resolutionU;
					
					float radius = Radius.GetValueAt(new Vector2(u, v));
					
					// Calculate the position of the rings of vertices:
					Vector3 surfaceNormal = (float)Math.Cos(u)*curveNormal + (float)Math.Sin(u)*curveBinormal;
					Vector3 surfacePosition = translation + radius*surfaceNormal;
					
					output.Add(new Vertex(surfacePosition, surfaceNormal));
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
					output.Add(indexOffset + i + resolutionU*j);
					output.Add(indexOffset + i + resolutionU*(j+1));
					output.Add(indexOffset + (i+1) + resolutionU*j);

					output.Add(indexOffset + i + resolutionU*(j+1));
					output.Add(indexOffset + (i+1) + resolutionU*(j+1));
					output.Add(indexOffset + (i+1) + resolutionU*j);
				}
				
				// Stitch the end of the triangles:
				output.Add(indexOffset + resolutionU-1 + resolutionU*j);
				output.Add(indexOffset + resolutionU-1 + resolutionU*(j+1));
				output.Add(indexOffset + resolutionU*j);

				output.Add(indexOffset + resolutionU-1 + resolutionU*(j+1));
				output.Add(indexOffset + resolutionU*(j+1));
				output.Add(indexOffset + resolutionU*j);
			}

			return output;
		}
	}
}
