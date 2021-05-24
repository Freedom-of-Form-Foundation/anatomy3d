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
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	/// 	RayCastDirection defines whether a ray is cast outwards from a surface or inwards into the surface.
	/// </summary>
	public enum RayCastDirection
	{
		Outwards,
		Inwards,
	}

	/// <summary>
	/// 	A MoldCastMap projects one surface onto a mold surface, giving the distance between the two surfaces
	///		on each coordinate point. This map can for example be used to shape the Cylinder and the Capsule surfaces.
	/// </summary>
	public class MoldCastMap : ContinuousMap<Vector2, float>
	{
		/// <summary>
		/// 	The surface from which the rays are cast. A ray is cast out of each point on the surface in the
		/// 	direction of the surface's normal.
		/// </summary>
		private readonly Surface raycastSurface;
		
		/// <summary>
		/// 	The surface that defines the mold. Rays that are cast are checked whether they intersect this surface.
		/// </summary>
		private readonly IRaytraceableSurface moldSurface;
		
		/// <summary>
		/// 	The height map that is returned when a ray does not intersect the moldSurface. This happens when the ray
		/// 	has missed the mold surface and shoots off to infinity. When that happens, return the length of
		/// 	defaultRadius instead, so that the heightmap is still defined.
		/// </summary>
		private readonly ContinuousMap<Vector2, float> defaultRadius;
		
		/// <summary>
		/// 	The direction along the normal of the raycastSurface from which to cast each ray. This could either be
		/// 	outwards from the surface, or in the opposite direction.
		/// </summary>
		private readonly RayCastDirection direction;
		
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
		public MoldCastMap(Curve raycastCurve,
								IRaytraceableSurface moldSurface,
								ContinuousMap<Vector2, float> defaultRadius,
								RayCastDirection direction = RayCastDirection.Outwards)
			: this(new Capsule(raycastCurve, 0.0f), moldSurface, defaultRadius, direction)
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
		public MoldCastMap(Surface raycastSurface,
								IRaytraceableSurface moldSurface,
								ContinuousMap<Vector2, float> defaultRadius,
								RayCastDirection direction = RayCastDirection.Outwards)
		{
			this.raycastSurface = raycastSurface;
			this.moldSurface = moldSurface;
			this.defaultRadius = defaultRadius;
			this.direction = direction;
		}
		
		/// <inheritdoc />
		public override float GetValueAt(Vector2 uv)
		{
			float intersectionRadius = moldSurface.RayIntersect(raycastSurface.GetPositionAt(uv),
																Vector3.Normalize(raycastSurface.GetNormalAt(uv)),
																direction);
			
			if (Single.IsInfinity(intersectionRadius) || Single.IsNaN(intersectionRadius))
			{
				return defaultRadius.GetValueAt(uv);
			} else {
				return intersectionRadius;
			}
		}
	}
}
