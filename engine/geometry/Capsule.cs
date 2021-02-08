using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public class Capsule : Surface
	{
		Cylinder shaft;
		Hemisphere startCap;
		Hemisphere endCap;
		
		public Capsule(Cylinder shaft)
		{
			this.shaft = shaft;
			this.startCap = new Hemisphere(
									shaft.Radius.GetValueAt(0.0f),
									shaft.CenterCurve.GetStartPosition(),
									-shaft.CenterCurve.GetTangentAt(0.0f)
								);
			this.endCap = new Hemisphere(
									shaft.Radius.GetValueAt(1.0f),
									shaft.CenterCurve.GetEndPosition(),
									shaft.CenterCurve.GetTangentAt(1.0f)
								);
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
