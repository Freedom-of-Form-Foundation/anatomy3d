/*
 * Copyright (C) 2021 Freedom of Form Foundation, Inc.
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License, version 2 (GPLv2) as published by the Free Software Foundation.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License, version 2 (GPLv2) for more details.
 * 
 * You should have received a copy of the GNU General Public License, version 2 (GPLv2)
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	/// <summary>
	/// 	A <c>Capsule</c> represents a (bent) cylindrical surface around a <c>Curve</c> with hemispherical end caps,
	/// 	that can be deformed by a heightmap at each point on the surface. In effect, the heightmap defines
	/// 	a radius at each point on the cylindrical surface.
	/// </summary>
	public class Capsule : Surface
	{
		protected Cylinder shaft;
		protected Hemisphere startCap;
		protected Hemisphere endCap;
		
#region Constructors
		/// <summary>
		/// 	Construct a new <c>Capsule</c> around a central curve, the <c>centerCurve</c>.
		/// 	The radius at each point on the central axis is defined by a one-dimensional function <c>radius</c>.
		/// 	
		/// 	The surface is parametrized with the coordinates \f$u, \phi\f$, with \f$u\f (along the length of the
		/// 	shaft) and \f$\phi \in [0, 2 \pi]\f$ along the radial coordinate. A <c>Capsule</c> consists of a
		/// 	cylindrical shaft with two hemisphere end caps. $\f$u \in [-\frac{1}{2} \pi, 0]\f$ marks the region
		/// 	in parametrized coordinates of the hemisphere at the start point of the cylinder,
		/// 	$\f$u \in [0, 1]\f$ marks the region in parametrized coordinates of the central shaft,
		/// 	and $\f$u \in [1, 1 + \frac{1}{2} \pi]\f$ marks the region in parametrized coordinates of the hemisphere
		/// 	at the end point of the cylinder. The <c>heightMap</c> is a two-dimensional function defined on these
		/// 	coordinates on the domain given above.
		/// </summary>
		/// <param name="centerCurve">
		/// 	<inheritdoc cref="Capsule.CenterCurve"/>
		/// </param>
		/// <param name="heightMap">
		/// 	The radius at each point on the surface. A <c>Capsule</c> is generated around a central curve, with each
		/// 	point on the surface at a certain distance from the curve defined by <c>heightMap</c>. <c>heightMap</c>
		/// 	is therefore a two-dimensional function $h(u, \phi)$ that outputs a distance from the curve at each of
		/// 	the surface's parametric coordinates.
		/// </param>
		public Capsule(Curve centerCurve, ContinuousMap<Vector2, float> heightMap)
		{
			Vector3 startTangent = -centerCurve.GetTangentAt(0.0f);
			Vector3 startNormal = centerCurve.GetNormalAt(0.0f);
			
			Vector3 endTangent = centerCurve.GetTangentAt(1.0f);
			Vector3 endNormal = centerCurve.GetNormalAt(1.0f);
			
			this.shaft = new Cylinder(centerCurve, heightMap);
			this.startCap = new Hemisphere(
									new ShiftedMap2D<float>(new Vector2(0.0f, -0.5f * (float)Math.PI), heightMap),
									centerCurve.GetStartPosition(),
									startTangent,
									startNormal,
									-Vector3.Cross(startTangent, startNormal)
								);
			this.endCap = new Hemisphere(
									new ShiftedMap2D<float>(new Vector2(0.0f, 1.0f + 0.5f * (float)Math.PI), new Vector2(1.0f, -1.0f), heightMap),
									centerCurve.GetEndPosition(),
									endTangent,
									endNormal,
									-Vector3.Cross(endTangent, endNormal)
								);
		}
#endregion Constructors

#region Properties
		/// <summary>
		/// 	The curve around which the capsule's cylinder shaft is generated.
		/// </summary>
		public Curve CenterCurve
		{
			get { return shaft.CenterCurve; }
		}
#endregion Properties
		
#region Base Class Method Overrides
		/// <inheritdoc />
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
				return endCap.GetNormalAt(new Vector2(u, 1.0f + 0.5f * (float)Math.PI - v));
			}
			else
			{
				throw new ArgumentOutOfRangeException("v","'v' must be between [-0.5 pi] and [1.0 + 0.5 pi].");
			}
		}
		
		/// <inheritdoc />
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
				return endCap.GetPositionAt(new Vector2(u, 1.0f + 0.5f * (float)Math.PI - v));
			}
			else
			{
				throw new ArgumentOutOfRangeException("v","'v' must be between [-0.5 pi] and [1.0 + 0.5 pi].");
			}
		}
		
		/// <inheritdoc />
		public override List<Vertex> GenerateVertexList(int resolutionU, int resolutionV)
		{
			// Generate the vertices of two end caps, and a shaft in between:
			List<Vertex> startCapList = startCap.GenerateVertexList(resolutionU, resolutionU/4);
			List<Vertex> endCapList = endCap.GenerateVertexList(resolutionU, resolutionU/4);
			
			// Using GetRange(), remove the first and last ring of vertices of the shaft, which overlap with the last
			// rings of the startCap and endCap respectively. Later we will stitch the shaft and end caps together with
			// triangles between them, during the GenerateIndexList() step.
			Slice<Vertex> shaftSlice = new Slice<Vertex>(
				shaft.GenerateVertexList(resolutionU, resolutionV),
				resolutionU,
				Cylinder.CalculateVertexCount(resolutionU, resolutionV) - 2*resolutionU);
			
			// Recalculate the surface normal between the cylinder and the start cap:
			for (int i = 0; i < (resolutionU - 1); i++)
			{
				Vector3 surfacePosition = startCapList[(resolutionU/4-1)*resolutionU + i + 1].Position;
				Vector3 du = surfacePosition - startCapList[(resolutionU/4-1)*resolutionU + i + 1 + 1].Position;
				Vector3 dv = surfacePosition - shaftSlice[i].Position;
				
				// Calculate the position of the rings of vertices:
				Vector3 surfaceNormal = Vector3.Cross(Vector3.Normalize(du), Vector3.Normalize(dv));
				
				startCapList[(resolutionU/4-1)*resolutionU + i + 1] = new Vertex(surfacePosition, surfaceNormal);
			}
			
			// Stitch the end of the triangles:
			Vector3 surfacePosition2 = startCapList[(resolutionU/4-1)*resolutionU + resolutionU].Position;
			Vector3 du2 = surfacePosition2 - startCapList[(resolutionU/4-1)*resolutionU + 1].Position;
			Vector3 dv2 = surfacePosition2 - shaftSlice[resolutionU].Position;
			
			// Calculate the position of the rings of vertices:
			Vector3 surfaceNormal2 = Vector3.Cross(Vector3.Normalize(du2), Vector3.Normalize(dv2));
			
			startCapList[(resolutionU/4-1)*resolutionU + resolutionU] = new Vertex(surfacePosition2, surfaceNormal2);
			
			return startCapList.Concat(shaftSlice).Concat(endCapList).ToList();
		}
		
		/// <inheritdoc />
		public override List<int> GenerateIndexList(int resolutionU, int resolutionV, int indexOffset = 0)
		{
			// The index list of a model tells the GPU between which vertices to draw triangles. Each three consecutive
			// indices constitute a single triangle. Since the Capsule vertex array consists of several lists stitched
			// together, the indices of the shaft and endCap will have shifted by some amount. Therefore, calculate
			// this index offset for all submodels:
			int indexOffset2 = indexOffset + Hemisphere.CalculateVertexCount(resolutionU, resolutionU/4) - resolutionU;
			int indexOffset3 = indexOffset2 + Cylinder.CalculateVertexCount(resolutionU, resolutionV) - resolutionU;
			int indexOffset4 = indexOffset3 + Hemisphere.CalculateVertexCount(resolutionU, resolutionU/4) - resolutionU;
			
			// Generate the startCap indices as normal:
			List<int> startCapList = startCap.GenerateIndexList(resolutionU, resolutionU/4, indexOffset);
			
			// Generate a shaft, using the last ring of the startCap to begin with. This is done by subtracting
			// `resolutionU` from indexOffset2. This automatically stitches the gap between the start cap and the shaft:
			List<int> shaftList = shaft.GenerateIndexList(resolutionU, resolutionV-1, indexOffset2);
			
			// Generate the endCap indices as normal:
			List<int> endCapList = endCap.GenerateIndexList(resolutionU, resolutionU/4, indexOffset3);
			
			// The final ring between the endCap and the shaft must be generated manually:
			List<int> stitchList = new List<int>(resolutionU);
			
			// Add a ring of triangles:
			for (int i = 1; i < resolutionU-1; i++)
			{
				int invertedI = resolutionU - i;
				stitchList.Add(indexOffset + invertedI + indexOffset4);
				stitchList.Add(indexOffset + (i+1) + indexOffset3 - resolutionU);
				stitchList.Add(indexOffset + i + indexOffset3 - resolutionU);

				stitchList.Add(indexOffset + (invertedI-1) + indexOffset4);
				stitchList.Add(indexOffset + (i+1) + indexOffset3 - resolutionU);
				stitchList.Add(indexOffset + invertedI + indexOffset4);
			}
			
			// Stitch the end of the ring of triangles:
			stitchList.Add(indexOffset + 1 + indexOffset4);
			stitchList.Add(indexOffset + indexOffset3 - resolutionU);
			stitchList.Add(indexOffset + resolutionU-1 + indexOffset3 - resolutionU);

			stitchList.Add(indexOffset + indexOffset4);
			stitchList.Add(indexOffset + indexOffset3 - resolutionU);
			stitchList.Add(indexOffset + 1 + indexOffset4);
			
			stitchList.Add(indexOffset + resolutionU-1 + indexOffset4);
			stitchList.Add(indexOffset + 1 + indexOffset3 - resolutionU);
			stitchList.Add(indexOffset + indexOffset3 - resolutionU);

			stitchList.Add(indexOffset + indexOffset4);
			stitchList.Add(indexOffset + resolutionU-1 + indexOffset4);
			stitchList.Add(indexOffset + indexOffset3 - resolutionU);
			
			return startCapList.Concat(shaftList).Concat(endCapList).Concat(stitchList).ToList();
		}
#endregion Base Class Method Overrides
	}
}
