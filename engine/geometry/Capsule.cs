using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public class Capsule : Surface
	{
		Cylinder shaft;
		Hemisphere startCap;
		Hemisphere endCap;
		
		public Capsule(Curve centerCurve, ContinuousMap<Vector2, float> heightMap)
		{
			this.shaft = new Cylinder(centerCurve, heightMap);
			this.startCap = new Hemisphere(
									new ShiftedMap2D<float>(new Vector2(0.0f, -0.5f * (float)Math.PI), heightMap),
									shaft.CenterCurve.GetStartPosition(),
									-shaft.CenterCurve.GetTangentAt(0.0f)
								);
			this.endCap = new Hemisphere(
									new ShiftedMap2D<float>(new Vector2(0.0f, 1.0f), heightMap),
									shaft.CenterCurve.GetEndPosition(),
									shaft.CenterCurve.GetTangentAt(1.0f)
								);
		}
		
		public override Vector3 GetNormalAt(Vector2 uv)
		{
			float u = uv.X;
			float v = uv.Y;
			
			if ((v >= -0.5f * (float)Math.PI) && (v < 0.0f))
			{
				return startCap.GetNormalAt(new Vector2(u, v + 0.5f * (float)Math.PI));
			}
			else if ((v >= 0.0f) && (v < 1.0f))
			{
				return shaft.GetNormalAt(new Vector2(u, v));
			}
			else if ((v >= 1.0f) && (v <= 1.0f + 0.5f * (float)Math.PI))
			{
				return endCap.GetNormalAt(new Vector2(u, v - 1.0f));
			}
			else
			{
				throw new ArgumentOutOfRangeException("v","'v' must be between [-0.5 pi] and [1.0 + 0.5 pi].");
			}
		}
		
		public override Vector3 GetPositionAt(Vector2 uv)
		{
			float u = uv.X;
			float v = uv.Y;
			
			if ((v >= -0.5f * (float)Math.PI) && (v < 0.0f))
			{
				return startCap.GetPositionAt(new Vector2(u, v + 0.5f * (float)Math.PI));
			}
			else if ((v >= 0.0f) && (v < 1.0f))
			{
				return shaft.GetPositionAt(new Vector2(u, v));
			}
			else if ((v >= 1.0f) && (v <= 1.0f + 0.5f * (float)Math.PI))
			{
				return endCap.GetPositionAt(new Vector2(u, v - 1.0f));
			}
			else
			{
				throw new ArgumentOutOfRangeException("v","'v' must be between [-0.5 pi] and [1.0 + 0.5 pi].");
			}
		}
		
		public override List<Vertex> GenerateVertexList(int resolutionU, int resolutionV)
		{
			List<Vertex> startCapList = startCap.GenerateVertexList(resolutionU, resolutionU/4);
			List<Vertex> shaftList = shaft.GenerateVertexList(resolutionU, resolutionV);
			List<Vertex> endCapList = endCap.GenerateVertexList(resolutionU, resolutionU/4);
			
			return startCapList.Concat(shaftList).Concat(endCapList).ToList();
		}
		
		public override List<int> GenerateIndexList(int resolutionU, int resolutionV, int indexOffset = 0)
		{
			int indexOffset2 = indexOffset + Hemisphere.CalculateVertexCount(resolutionU, resolutionU/4);
			int indexOffset3 = indexOffset + indexOffset2 + Cylinder.CalculateVertexCount(resolutionU, resolutionV);
			
			List<int> startCapList = startCap.GenerateIndexList(resolutionU, resolutionU/4, indexOffset);
			List<int> shaftList = shaft.GenerateIndexList(resolutionU, resolutionV, indexOffset2);
			List<int> endCapList = endCap.GenerateIndexList(resolutionU, resolutionU/4, indexOffset3);
			
			return startCapList.Concat(shaftList).Concat(endCapList).ToList();
		}

		public Curve GetCenterCurve() {
			return shaft.CenterCurve;
		}
	}
}
