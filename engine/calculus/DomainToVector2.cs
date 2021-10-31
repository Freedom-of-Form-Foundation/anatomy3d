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
	///     <c>DomainToVector2<TOut></c> allows a function on a lower-dimensional domain to be used in places that
	///		expect a higher-dimensional domain. For example, one might want to use a 1D function to describe the
	///		height of a 2D plane along just a single axis (such as the shape of a corrugated sheet). Since the 2D
	///		plane expects a 2D function to describe its height, one cannot use a 1D function to describe its height
	///		properly, since it is unknown which axis the function should use. <c>DomainToVector2<TOut></c> solves this
	///		issue by stretching a 1D function onto a 2D domain.
	/// </summary>
	/// <param name="direction">
	/// 	The direction in which the 1D function is placed onto the 2D domain.
	/// </param>
	public class DomainToVector2<TOut> : ContinuousMap<dvec2, TOut>
	{
		public dvec2 ParameterDirection { get; set; }
		public ContinuousMap<double, TOut> Function { get; set; }
		
		/// <summary>
		///		Constructs a ContinuousMap taking 2D input values, based on a ContinuousMap that takes 1D input values.
		/// 	<example>For example:
		/// 	<code>
		///			// Define an arbitrary 1D function:
		/// 		ContinuousMap<float, float> lineFunction = new QuadraticFunction(1.0f, 1.0f, 1.0f);
		///			
		///			Vector2 direction = new Vector2(0.0f, 1.0f); // Let the 2D function vary along the y-axis.
		/// 		ContinuousMap<Vector2, float> planeFunction = new DomainToVector2<float>(direction, function);
		/// 	</code>
		/// 	creates a 2D function described by a 1D quadratic function along the y-axis. The x-axis of the resulting
		///		function is therefore constant, while the output value varies along the y-axis, as specified by
		///		<c>direction</c>.
		/// 	</example>
		/// </summary>
		/// <param name="direction">
		/// 	The direction in which the 1D function is placed onto the 2D domain. The magnitude of this vector
		/// 	has an effect on the scale of the function.
		/// </param>
		public DomainToVector2(dvec2 direction, ContinuousMap<double, TOut> function)
		{
			ParameterDirection = direction;
			Function = function;
		}
		
		/// <inheritdoc />
		public override TOut GetValueAt(dvec2 t)
		{
			return Function.GetValueAt(dvec2.Dot(ParameterDirection, t));
		}
	}
}
