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
using System;
using GlmSharp;

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
		public Hemisphere(ContinuousMap<dvec2, double> radius, dvec3 center, dvec3 direction)
			: this(radius,
					center,
					direction,
					dvec3.Cross(direction, dvec3.UnitZ),
					dvec3.Cross(direction, dvec3.Cross(direction, dvec3.UnitZ).Normalized)
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
		public Hemisphere(ContinuousMap<dvec2, double> radius, dvec3 center, dvec3 direction, dvec3 normal, dvec3 binormal)
		{
			this.Radius = radius;
			this.Center = center;
			this.Direction = direction;
			this.Normal = normal;
			this.Binormal = binormal;
		}
#endregion
		
#region Properties
		private ContinuousMap<dvec2, double> Radius { get; set; }
		
		/// <summary>
		/// 	The position of the hemisphere.
		/// </summary>
		public dvec3 Center { get; set; }
		
		private dvec3 _direction;
		/// <summary>
		/// 	The vector that defines which way is 'forward' for the object. Will be normalized on initialization.
		/// </summary>
		public dvec3 Direction
		{
			get { return _direction; }
			set { _direction = value.Normalized; }
		}
		
		private dvec3 _pointNormal;
		/// <summary>
		/// 	The vector that defines which way is 'up' for the object. Should be perpendicular to
		/// 	<c>direction</c> and <c>binormal</c>. Will be normalized on initialization.
		/// </summary>
		public dvec3 Normal
		{
			get { return _pointNormal; }
			set { _pointNormal = value.Normalized; }
		}
		private dvec3 _pointBinormal;
		
		/// <summary>
		/// 	The vector that defines which way is 'left' for the object. Should be perpendicular to
		/// 	<c>direction</c> and <c>normal</c>. Will be normalized on initialization.
		/// </summary>
		public dvec3 Binormal
		{
			get { return _pointBinormal; }
			set { _pointBinormal = value.Normalized; }
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
		public override dvec3 GetNormalAt(dvec2 uv)
		{
			double u = uv.x;
			double v = uv.y;
			
			// Calculate the position of the rings of vertices:
			double x = Math.Sin(v) * Math.Cos(u);
			double y = Math.Sin(v) * Math.Sin(u);
			double z = Math.Cos(v);
			
			return x*Normal + y*Binormal + z*Direction;
		}
		
		/// <inheritdoc />
		public override dvec3 GetPositionAt(dvec2 uv)
		{
			dvec3 translation = Center;
			double radius = this.Radius.GetValueAt(uv);
			
			return translation + radius*GetNormalAt(uv);
		}
		
		/// <inheritdoc />
		public override List<Vertex> GenerateVertexList(int resolutionU, int resolutionV)
		{
			List<Vertex> output = new List<Vertex>(CalculateVertexCount(resolutionU, resolutionV));
			
			// Load all required variables:
			dvec3 up = dvec3.UnitZ;
			
			dvec3 pointTangent = Direction;
			dvec3 pointNormal = Normal;
			dvec3 pointBinormal = Binormal;
			
			dvec3 translation = Center;

			// Get the radius at the top of the hemisphere:
			double topRadius = Radius.GetValueAt(new dvec2(0.0, 0.5 * Math.PI));

			// Generate the first point at the pole of the hemisphere:
			output.Add(new Vertex((vec3)(translation + topRadius * pointTangent), (vec3)pointTangent));

			// Generate rings of the other points:
			for (int j = 1; j < (resolutionV+1); j++)
			{
				for (int i = 0; i < resolutionU; i++)
				{
					// First find the normalized uv-coordinates, u = [0, 2pi], v = [0, 1/2pi]:
					double u = i/(double)resolutionU * 2.0 * Math.PI;
					double v = j/(double)resolutionV * 0.5 * Math.PI;
					
					// Calculate the position of the rings of vertices:
					double x = Math.Sin(v) * Math.Cos(u);
					double y = Math.Sin(v) * Math.Sin(u);
					double z = Math.Cos(v);
					
					double radius = Radius.GetValueAt(new dvec2(u, v));
					
					dvec3 surfaceNormal = x*pointNormal + y*pointBinormal + z*pointTangent;
					dvec3 surfacePosition = translation + radius*surfaceNormal;
					
					output.Add(new Vertex((vec3)surfacePosition, (vec3)surfaceNormal));
				}
			}
			
			// Recalculate the surface normal after deformation:
			for (int j = 1; j < resolutionV; j++)
			{
				for (int i = 0; i < (resolutionU - 1); i++)
				{
					dvec3 surfacePosition = output[(j-1)*resolutionU + i + 1].Position;
					dvec3 du = surfacePosition - output[(j-1)*resolutionU + i + 1 + 1].Position;
					dvec3 dv = surfacePosition - output[(j)*resolutionU + i + 1].Position;
					
					// Calculate the position of the rings of vertices:
					dvec3 surfaceNormal = dvec3.Cross(du.Normalized, dv.Normalized);
					
					output[(j-1)*resolutionU + i + 1] = new Vertex((vec3)surfacePosition, (vec3)surfaceNormal);
				}
				
				// Stitch the end of the triangles:
				dvec3 surfacePosition2 = output[(j-1)*resolutionU + resolutionU].Position;
				dvec3 du2 = surfacePosition2 - output[(j-1)*resolutionU + 1].Position;
				dvec3 dv2 = surfacePosition2 - output[(j)*resolutionU + resolutionU].Position;
				
				// Calculate the position of the rings of vertices:
				dvec3 surfaceNormal2 = dvec3.Cross(du2.Normalized, dv2.Normalized);
				
				output[(j-1)*resolutionU + resolutionU] = new Vertex((vec3)surfacePosition2, (vec3)surfaceNormal2);
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
