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

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	/// 	<c>ShiftedMap2D<TOut></c> shifts the domain of a 2D map such that the values are taken at
	/// 	`Shift + dvec2.Multiply(Stretch, uv)`.
	/// </summary>
	public class ShiftedMap2D<TOut> : ContinuousMap<dvec2, TOut>
	{
		/// <summary>
		/// 	The vector by which the 2D map is shifted.
		/// </summary>
		public dvec2 Shift { get; set; }
		
		/// <summary>
		/// 	The vector by which the 2D map's coordinates are multiplied.
		/// </summary>
		public dvec2 Stretch { get; set; }
		
		/// <summary>
		/// 	The ContinuousMap that is shifted and stretched.
		/// </summary>
		public ContinuousMap<dvec2, TOut> Function { get; set; }
		
		/// <summary>
		///		Constructs a ShiftedMap2D whose input is shifted by a dvec2.
		/// </summary>
		/// <param name="shift">
		/// 	The vector by which the 2D map is shifted.
		/// </param>
		/// <param name="function">
		/// 	The ContinuousMap that is shifted and stretched.
		/// </param>
		public ShiftedMap2D(dvec2 shift, ContinuousMap<dvec2, TOut> function)
			: this(shift, new dvec2(1.0, 1.0), function)
		{
			
		}
		
		/// <summary>
		///		Constructs a ShiftedMap2D whose input is shifted by a dvec2 and stretched by another dvec2.
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
		public ShiftedMap2D(dvec2 shift, dvec2 stretch, ContinuousMap<dvec2, TOut> function)
		{
			this.Shift = shift;
			this.Stretch = stretch;
			this.Function = function;
		}
		
		/// <inheritdoc />
		public override TOut GetValueAt(dvec2 uv)
		{
			return Function.GetValueAt(Shift + Stretch * uv);
		}
	}
}
