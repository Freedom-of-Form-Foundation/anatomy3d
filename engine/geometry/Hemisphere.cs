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
using System;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	/// <summary>
	/// 	A <c>Hemisphere</c> represents a hemispherical surface parametrized in spherical coordinates, and can have
	/// 	a varying radius on each point on the hemisphere by using a radial heightmap.
	/// </summary>
	public class Hemisphere : Surface
	{

#region Constructors
		/// <summary>
		/// 	Construct a new <c>Hemisphere</c> at the point <c>center</c>, pointing in the direction of
		/// 	<c>direction</c>. The radius is defined by a two-dimensional function <c>radius</c>.
		/// 	
		/// 	The surface is parametrized with the coordinates \f$u \in [0, 2\pi]\f$ (the azimuthal angle) and
		/// 	\f$v \in [0, \frac{1}{2} \pi]\f$ (the inclination angle).
		/// </summary>
		/// <param name="center">
		/// 	The position of the hemisphere.
		/// </param>
		/// <param name="direction">
		/// 	The vector that defines which way is 'forward' for the object. Will be normalized on initialization.
		/// </param>
		public Hemisphere(ContinuousMap<Vector2, float> radius, Vector3 center, Vector3 direction)
			: this(radius,
					center,
					direction,
					Vector3.Cross(direction, Vector3.UnitZ),
					Vector3.Cross(direction, Vector3.Normalize(Vector3.Cross(direction, Vector3.UnitZ)))
				)
		{
			
		}
		
		/// <summary>
		/// 	Construct a new <c>Hemisphere</c> at the point <c>center</c>, pointing in the direction of
		/// 	<c>direction</c>. The radius is defined by a two-dimensional function <c>radius</c>.
		/// 	
		/// 	The surface is parametrized with the coordinates \f$u \in [0, 2\pi]\f$ (the azimuthal angle) and
		/// 	\f$v \in [0, \frac{1}{2} \pi]\f$ (the inclination angle).
		/// </summary>
		/// <param name="center">
		/// 	The position of the hemisphere.
		/// </param>
		/// <param name="direction">
		/// 	The vector that defines which way is 'forward' for the object. Will be normalized on initialization.
		/// </param>
		/// <param name="normal">
		/// 	The vector that defines which way is 'up' for the object. Should be perpendicular to
		/// 	<c>direction</c> and <c>binormal</c>. Will be normalized on initialization.
		/// </param>
		/// <param name="binormal">
		/// 	The vector that defines which way is 'left' for the object. Should be perpendicular to
		/// 	<c>direction</c> and <c>normal</c>. Will be normalized on initialization.
		/// </param>
		public Hemisphere(ContinuousMap<Vector2, float> radius, Vector3 center, Vector3 direction, Vector3 normal, Vector3 binormal)
		{
			this.Radius = radius;
			this.Center = center;
			this.Direction = direction;
			this.Normal = normal;
			this.Binormal = binormal;
		}
#endregion
		
#region Properties
		private ContinuousMap<Vector2, float> Radius { get; set; }
		
		/// <summary>
		/// 	The position of the hemisphere.
		/// </summary>
		public Vector3 Center { get; set; }
		
		private Vector3 direction;
		/// <summary>
		/// 	The vector that defines which way is 'forward' for the object. Will be normalized on initialization.
		/// </summary>
		public Vector3 Direction
		{
			get { return direction; }
			set { direction = Vector3.Normalize(value); }
		}
		
		private Vector3 pointNormal;
		/// <summary>
		/// 	The vector that defines which way is 'up' for the object. Should be perpendicular to
		/// 	<c>direction</c> and <c>binormal</c>. Will be normalized on initialization.
		/// </summary>
		public Vector3 Normal
		{
			get { return pointNormal; }
			set { pointNormal = Vector3.Normalize(value); }
		}
		private Vector3 pointBinormal;
		
		/// <summary>
		/// 	The vector that defines which way is 'left' for the object. Should be perpendicular to
		/// 	<c>direction</c> and <c>normal</c>. Will be normalized on initialization.
		/// </summary>
		public Vector3 Binormal
		{
			get { return pointBinormal; }
			set { pointBinormal = Vector3.Normalize(value); }
		}
#endregion

#region Static Methods
		/// <summary>
		/// 	Returns the amount of vertices that the hemisphere would have at a certain resolution.
		/// </summary>
		public static int CalculateVertexCount(int resolutionU, int resolutionV)
		{
			return resolutionU * resolutionV + 1;
		}
		
		/// <summary>
		/// 	Returns the amount of indices that the hemisphere would have at a certain resolution.
		/// </summary>
		public static int CalculateIndexCount(int resolutionU, int resolutionV)
		{
			return resolutionU * resolutionV * 6 + resolutionU * 3;
		}
#endregion

#region Base Class Method Overrides
		/// <inheritdoc />
		public override Vector3 GetNormalAt(Vector2 uv)
		{
			float u = uv.X;
			float v = uv.Y;
			
			// Calculate the position of the rings of vertices:
			float x = (float)Math.Sin(v) * (float)Math.Cos(u);
			float y = (float)Math.Sin(v) * (float)Math.Sin(u);
			float z = (float)Math.Cos(v);
			
			return x*Normal + y*Binormal + z*Direction;
		}
		
		/// <inheritdoc />
		public override Vector3 GetPositionAt(Vector2 uv)
		{
			Vector3 translation = Center;
			float radius = this.Radius.GetValueAt(uv);
			
			return translation + radius*GetNormalAt(uv);
		}
		
		/// <inheritdoc />
		public override List<Vertex> GenerateVertexList(int resolutionU, int resolutionV)
		{
			List<Vertex> output = new List<Vertex>(CalculateVertexCount(resolutionU, resolutionV));
			
			// Load all required variables:
			Vector3 up = new Vector3(0.0f, 0.0f, 1.0f);
			
			Vector3 pointTangent = Direction;
			Vector3 pointNormal = Normal;
			Vector3 pointBinormal = Binormal;
			
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
			
			// Recalculate the surface normal after deformation:
			for (int j = 1; j < resolutionV; j++)
			{
				for (int i = 0; i < (resolutionU - 1); i++)
				{
					Vector3 surfacePosition = output[(j-1)*resolutionU + i + 1].Position;
					Vector3 du = surfacePosition - output[(j-1)*resolutionU + i + 1 + 1].Position;
					Vector3 dv = surfacePosition - output[(j)*resolutionU + i + 1].Position;
					
					// Calculate the position of the rings of vertices:
					Vector3 surfaceNormal = Vector3.Cross(Vector3.Normalize(du), Vector3.Normalize(dv));
					
					output[(j-1)*resolutionU + i + 1] = new Vertex(surfacePosition, surfaceNormal);
				}
				
				// Stitch the end of the triangles:
				Vector3 surfacePosition2 = output[(j-1)*resolutionU + resolutionU].Position;
				Vector3 du2 = surfacePosition2 - output[(j-1)*resolutionU + 1].Position;
				Vector3 dv2 = surfacePosition2 - output[(j)*resolutionU + resolutionU].Position;
				
				// Calculate the position of the rings of vertices:
				Vector3 surfaceNormal2 = Vector3.Cross(Vector3.Normalize(du2), Vector3.Normalize(dv2));
				
				output[(j-1)*resolutionU + resolutionU] = new Vertex(surfacePosition2, surfaceNormal2);
			}
			
			return output;
		}
		
		/// <inheritdoc />
		public override List<int> GenerateIndexList(int resolutionU, int resolutionV, int indexOffset = 0)
		{
			List<int> output = new List<int>(CalculateIndexCount(resolutionU, resolutionV));
			
			// Add a triangle fan on the pole of the hemisphere:
			for (int i = 0; i < resolutionU-1; i++)
			{
				output.Add(indexOffset + 1 + i);
				output.Add(indexOffset + 2 + i);
				output.Add(indexOffset + 0);
			}
			
			// Stitch final triangle on the pole of the hemisphere:
			output.Add(indexOffset + resolutionU);
			output.Add(indexOffset + 1);
			output.Add(indexOffset + 0);
			
			// Add the remaining rings:
			for (int j = 0; j < resolutionV - 1; j++)
			{
				// Add a ring of triangles:
				for (int i = 0; i < resolutionU-1; i++)
				{
					output.Add(indexOffset + 1 + i + resolutionU*(j+1));
					output.Add(indexOffset + 1 + (i+1) + resolutionU*j);
					output.Add(indexOffset + 1 + i + resolutionU*j);

					output.Add(indexOffset + 1 + (i+1) + resolutionU*(j+1));
					output.Add(indexOffset + 1 + (i+1) + resolutionU*j);
					output.Add(indexOffset + 1 + i + resolutionU*(j+1));
				}
				
				// Stitch the end of the ring of triangles:
				output.Add(indexOffset + 1 + resolutionU-1 + resolutionU*(j+1));
				output.Add(indexOffset + 1 + resolutionU*j);
				output.Add(indexOffset + 1 + resolutionU-1 + resolutionU*j);

				output.Add(indexOffset + 1 + resolutionU*(j+1));
				output.Add(indexOffset + 1 + resolutionU*j);
				output.Add(indexOffset + 1 + resolutionU-1 + resolutionU*(j+1));
			}

			return output;
		}
#endregion
	}
}
