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
	/// 	A <c>CorrugatedPlane</c> represents a planar surface that can be deformed by a heightmap. Due to its simplicity, it is
	/// 	raytraceable.
	/// </summary>
	public class CorrugatedPlane : IRaytraceableSurface
	{
		#region Constructors
		/// <summary>
		/// 	Construct a new <c>CorrugatedPlane</c> at the point <c>Origin</c>. The vectors <c>VectorU</c> and
		/// 	<c>VectorV</c> span the plane in the u and v-coordinate, respectively, such that <c>VectorU</c> and
		/// 	<c>VectorV</c> together form a parallelogram that defines the plane. This plane is deformed by
		/// 	<c>heightmap</c>.
		/// 	
		/// 	The surface is parametrized with the coordinates \f$u \in [0, 1]\f$ (along the <c>VectorU</c> direction)
		/// 	and \f$v \in [0, 1]\f$ (along the <c>VectorV</c> direction).
		/// </summary>
		/// <param name="vectorU">
		/// 	One of the two vectors defining the span and size of the plane. The u-coordinate follows this vector.
		/// </param>
		/// <param name="vectorV">
		/// 	One of the two vectors defining the span and size of the plane. The v-coordinate follows this vector.
		/// </param>
		/// <param name="origin">
		/// 	The starting point in 3D space of the plane. This is where the parametric coordinates (u,v) are (0,0).
		/// </param>
		/// <param name="heightmap">
		/// 	The heightmap that deforms the plane in the normal direction. The normal is defined as the normalized
		/// 	cross product between <c>VectorU</c> and <c>VectorV</c>.
		/// </param>
		public CorrugatedPlane(dvec3 origin, dvec3 vectorU, dvec3 vectorV, RaytraceableFunction1D heightmap, double heightFactor = 1.0)
		{
			HeightFactor = heightFactor;
			heightmap1D = heightmap;
			VectorU = vectorU;
			VectorV = vectorV;
			Origin = origin;
		}
		#endregion

		#region Properties
		// TODO: change access modifier
		protected RaytraceableFunction1D heightmap1D;


		private dvec3 _vectorU;
		public dvec3 VectorU {
			get { return _vectorU; }
			set
			{
				_vectorU = value;
				_invertedPlaneMatrix = new dmat3(VectorU, VectorV, Normal).Inverse;
			}
		}

		private dvec3 _vectorV;
		public dvec3 VectorV
		{
			get { return _vectorV; }
			set
			{
				_vectorV = value;
				_invertedPlaneMatrix = new dmat3(VectorU, VectorV, Normal).Inverse;
			}
		}

		public dvec3 Origin { get; set; }
		public dvec3 Normal
		{
			get
			{
				return HeightFactor * dvec3.Cross(VectorU, VectorV).Normalized;
			}
		}

		public double HeightFactor { get; set; }
		#endregion Properties

		#region Private Variables
		private dmat3 _invertedPlaneMatrix;
		#endregion Private Variables

		#region IRaytraceableSurface
		/// <inheritdoc />
		public RaySurfaceIntersection RayIntersect(Ray ray)
		{ 
			dvec3 r0 = _invertedPlaneMatrix*(ray.StartPosition - Origin);
			dvec3 r1 = _invertedPlaneMatrix*(ray.Direction);

			IEnumerable<double> intersections = heightmap1D.SolveRaytracePlanar(r0, r1);
			
			// The previous function returns a list of intersection distances. The value closest to 0.0f represents the
			// closest intersection point.
			RaySurfaceIntersection minimum = new RaySurfaceIntersection(Single.PositiveInfinity, 0.0, 0.0);

			foreach (double i in intersections)
			{
				// First calculate the parameters u and v on the surface of the Plane at which the ray intersects.
				double u = r0.x + i * r1.x;
				double v = r0.y + i * r1.y;

				// t must be within the bounds of the Plane. t ranges from 0.0 to 1.0, so if it is outside
				// of this region, the ray does not intersect the cylinder.
				if (!(u >= 0.0 && u <= 1.0) || !(v >= 0.0 && v <= 1.0))
				{
					break;
				}

				// Determine if the ray is inside the pie-slice of the cylinder that is being displayed,
				// otherwise discard:
				if (i >= 0.0)
				{
					if (Math.Abs(i) < Math.Abs(minimum.RayLength))
					{
						minimum.RayLength = i;
						minimum.U = u;
						minimum.V = v;
					}
				}
			}
			
			return minimum;
		}
		#endregion IRaytraceableSurface
	}
}
