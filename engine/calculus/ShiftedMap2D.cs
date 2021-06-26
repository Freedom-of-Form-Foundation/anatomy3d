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

using System.Numerics;
using System;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	/// 	<c>ShiftedMap2D<TOut></c> shifts the domain of a 2D map such that the values are taken at
	/// 	`Shift + Vector2.Multiply(Stretch, uv)`.
	/// </summary>
	public class ShiftedMap2D<TOut> : ContinuousMap<Vector2, TOut>
	{
		/// <summary>
		/// 	The vector by which the 2D map is shifted.
		/// </summary>
		public Vector2 Shift { get; set; }
		
		/// <summary>
		/// 	The vector by which the 2D map's coordinates are multiplied.
		/// </summary>
		public Vector2 Stretch { get; set; }
		
		/// <summary>
		/// 	The ContinuousMap that is shifted and stretched.
		/// </summary>
		public ContinuousMap<Vector2, TOut> Function { get; set; }
		
		/// <summary>
		///		Constructs a ShiftedMap2D whose input is shifted by a Vector2.
		/// </summary>
		/// <param name="shift">
		/// 	The vector by which the 2D map is shifted.
		/// </param>
		/// <param name="function">
		/// 	The ContinuousMap that is shifted and stretched.
		/// </param>
		public ShiftedMap2D(Vector2 shift, ContinuousMap<Vector2, TOut> function)
			: this(shift, new Vector2(1.0f, 1.0f), function)
		{
			
		}
		
		/// <summary>
		///		Constructs a ShiftedMap2D whose input is shifted by a Vector2 and stretched by another Vector2.
		/// </summary>
		/// <param name="shift">
		/// 	The vector by which the 2D map is shifted.
		/// </param>
		/// <param name="stretch">
		/// 	The vector by which the 2D map's coordinates are multiplied.
		/// </param>
		/// <param name="function">
		/// 	The ContinuousMap that is shifted and stretched.
		/// </param>
		public ShiftedMap2D(Vector2 shift, Vector2 stretch, ContinuousMap<Vector2, TOut> function)
		{
			this.Shift = shift;
			this.Stretch = stretch;
			this.Function = function;
		}
		
		/// <inheritdoc />
		public override TOut GetValueAt(Vector2 uv)
		{
			return Function.GetValueAt(Shift + Vector2.Multiply(Stretch, uv));
		}
	}
}
