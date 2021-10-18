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

using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	/// <summary>
	///     A <c>Curve</c> defines any finite one-dimensional path in 3D space, with a start and end point.
	/// </summary>
	public abstract class Curve : ContinuousMap<double, dvec3>
	{
		/// <summary>
		///     Returns the position in 3D space of the curve based on the parametric variable <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The parameter along the length of the curve. <paramref name="t"/> must be in the range
		/// \f$[ 0, 1 ]\f$.</param>
		public abstract dvec3 GetPositionAt(double t);
		
		/// <summary>
		///     Returns the tangent vector of the curve based on the parametric variable <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The parameter along the length of the curve. <paramref name="t"/> must be in the range
		/// \f$[ 0, 1 ]\f$.</param>
		public abstract dvec3 GetTangentAt(double t);
		
		/// <summary>
		///     Returns the normal vector of the curve based on the parametric variable <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The parameter along the length of the curve. <paramref name="t"/> must be in the range
		/// \f$[ 0, 1 ]\f$.</param>
		public abstract dvec3 GetNormalAt(double t);
		
		/// <summary>
		///     Returns the binormal vector of the curve based on the parametric variable <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The parameter along the length of the curve. <paramref name="t"/> must be in the range
		/// \f$[ 0, 1 ]\f$.</param>
		public virtual dvec3 GetBinormalAt(double t)
		{
			return dvec3.Cross(GetTangentAt(t), GetNormalAt(t));
		}
		
		/// <summary>
		///     Returns the position in 3D space of the start point of the curve.
		///     Should return the same value as <c>getPositionAt(0.0)</c>.
		/// </summary>
		public virtual dvec3 GetStartPosition()
		{
			return GetPositionAt(0.0);
		}
		
		/// <summary>
		///     Returns the position in 3D space of the end point of the curve.
		///     Should return the same value as <c>getPositionAt(1.0)</c>.
		/// </summary>
		public virtual dvec3 GetEndPosition()
		{
			return GetPositionAt(1.0);
		}
		
		/// <inheritdoc />
		public override dvec3 GetValueAt(double t)
		{
			return GetPositionAt(t);
		}
	}
}
