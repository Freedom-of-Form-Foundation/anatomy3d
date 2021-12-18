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

using GlmSharp;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	/// <summary>
	///		A <c>Ray</c> defines a ray with a starting position and a direction, to be used in the raytracer.
	/// </summary>
	public struct Ray
	{
		/// <summary>
		///		The point from which the ray is cast.
		/// </summary>
		public dvec3 StartPosition { get; set; }
		
		dvec3 _direction;
		/// <summary>
		///		The direction in which the ray is cast. The direction becomes normalized on assignment.
		/// </summary>
		public dvec3 Direction {
			get => _direction;
			set => _direction = value.Normalized;
		}
		
		/// <summary>
		///		Defines a ray with a starting position and a direction, to be used in the raytracer.
		/// </summary>
		/// <param name="rayStart">
		///		The point from which the ray is cast.
		/// </param>
		/// <param name="rayDirection">
		///		The direction in which the ray is cast. The direction becomes normalized on initialization.
		/// </param>
		public Ray(dvec3 rayStart, dvec3 rayDirection)
		{
			StartPosition = rayStart;
			_direction = rayDirection.Normalized;
		}
	}
}
