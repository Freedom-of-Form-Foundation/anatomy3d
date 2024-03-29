﻿/*
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

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	/// <summary>
	///     An interface defining the ability to raytrace a surface analytically.
	/// </summary>
	public interface IRaytraceableSurface
	{
		/// <summary>
		///		Intersects the surface with a ray \f$\vec{p} + t \vec{s}\f$, with \f$t \geq 0\f$, returning the minimum
		/// 	\f$t\f$ for which there is an intersection. \f$\vec{p}\f$ is given by <c>ray.StartPosition</c>, and
		/// 	\f$\vec{s}\f$ is given by <c>ray.Direction</c>.
		/// </summary>
		/// <param name="ray">
		///		The Ray object that defines the starting point and direction of the ray to be cast.
		/// </param>
		double RayIntersect(Ray ray);
	}
}
