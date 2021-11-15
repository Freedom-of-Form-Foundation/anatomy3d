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

using System;
using GlmSharp;

using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	/// 	A MoldCastMap projects one surface onto a mold surface, giving the distance between the two surfaces
	///		on each coordinate point. This map can for example be used to shape the Cylinder and the Capsule surfaces.
	/// </summary>
	public class SmoothMoldCastMap : MoldCastMap
	{
		/// <summary>
		/// 	The surface that defines the mold. Rays that are cast are checked whether they intersect this surface.
		/// </summary>
		private new readonly IExtendedRaytraceableSurface moldSurface;
		
		/// <summary>
		/// 	Construct a new MoldCastMap from a curve.
		/// </summary>
		/// <param name="raycastCurve">
		/// 	The curve from which the rays are cast. A ray is cast out of each point on the curve. This could for
		/// 	example be the center curve of a Capsule, in which case the CurveMoldCastMap returns a height map that
		/// 	ensures that the Capsule is shaped like the moldSurface.
		/// </param>
		/// <param name="moldSurface">
		/// 	The surface that defines the mold that shapes the height map that is returned by the MoldCastMap. Rays
		/// 	that are cast are checked whether they intersect this surface.
		/// </param>
		/// <param name="defaultRadius">
		/// 	The height map that is returned when a ray does not intersect the moldSurface. This happens when the ray
		/// 	has missed the mold surface and shoots off to infinity. When that happens, return the length of
		/// 	defaultRadius instead, so that the heightmap is still defined.
		/// </param>
		/// <param name="direction">
		/// 	The direction along the normal of the raycastSurface from which to cast each ray. This could either be
		/// 	outwards from the surface, or in the opposite direction.
		/// </param>
		/// <param name="maxDistance">
		/// 	The maximum ray length before a ray is considered to be out of bounds.
		/// </param>
		public SmoothMoldCastMap(Curve raycastCurve,
								IExtendedRaytraceableSurface moldSurface,
								ContinuousMap<dvec2, double> defaultRadius,
								RayCastDirection direction = RayCastDirection.Outwards,
								double maxDistance = Double.PositiveInfinity)
			: this(new Capsule(raycastCurve, 0.0), moldSurface, defaultRadius, direction, maxDistance)
		{
			
		}
		
		/// <summary>
		/// 	Construct a new MoldCastMap.
		/// </summary>
		/// <param name="raycastSurface">
		/// 	The surface from which the rays are cast. A ray is cast out of each point on the surface in the
		/// 	direction of the surface's normal.
		/// </param>
		/// <param name="moldSurface">
		/// 	The surface that defines the mold that shapes the height map that is returned by the MoldCastMap. Rays
		/// 	that are cast are checked whether they intersect this surface.
		/// </param>
		/// <param name="defaultRadius">
		/// 	The height map that is returned when a ray does not intersect the moldSurface. This happens when the ray
		/// 	has missed the mold surface and shoots off to infinity. When that happens, return the length of
		/// 	defaultRadius instead, so that the heightmap is still defined.
		/// </param>
		/// <param name="direction">
		/// 	The direction along the normal of the raycastSurface from which to cast each ray. This could either be
		/// 	outwards from the surface, or in the opposite direction.
		/// </param>
		/// <param name="maxDistance">
		/// 	The maximum ray length before a ray is considered to be out of bounds.
		/// </param>
		public SmoothMoldCastMap(Surface raycastSurface,
								IExtendedRaytraceableSurface moldSurface,
								ContinuousMap<dvec2, double> defaultRadius,
								RayCastDirection direction = RayCastDirection.Outwards,
								double maxDistance = Double.PositiveInfinity)
			: base(raycastSurface, moldSurface, defaultRadius, direction, maxDistance)
		{
			this.moldSurface = moldSurface;
		}

		/// <inheritdoc />
		public override double GetValueAt(dvec2 uv)
		{
			double directionSign = (direction == RayCastDirection.Outwards) ? 1.0 : -1.0;
			
			Ray ray = new Ray(raycastSurface.GetPositionAt(uv), directionSign*raycastSurface.GetNormalAt(uv).Normalized);
			RayExtendedSurfaceIntersection intersection = moldSurface.ExtendedRayIntersect(ray);

			double x = intersection.DistanceToSurface;
			double pow = Math.Pow(x * 10.0, 3.0);
			double weight = pow / (1.0 + pow);

			if (Math.Abs(intersection.RayLength) <= maxDistance)
			{
				return intersection.RayLength * (1.0 - weight) + weight * defaultRadius.GetValueAt(uv);
			}
			else {
				return defaultRadius.GetValueAt(uv);
			}
		}
	}
}
