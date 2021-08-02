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

using FreedomOfFormFoundation.AnatomyEngine.Geometry;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Anatomy.Joints
{
	public class HingeJoint : Joint
	{
		SymmetricCylinder articularSurface;
		
		public HingeJoint(LineSegment centerLine, RaytraceableFunction1D radius)
		{
			articularSurface = new SymmetricCylinder(centerLine, radius);
		}
		
		public HingeJoint(LineSegment centerLine, RaytraceableFunction1D radius, ContinuousMap<double, double> startAngle, ContinuousMap<double, double> endAngle)
		{
			articularSurface = new SymmetricCylinder(centerLine, radius, startAngle, endAngle);
		}
		
		//public HingeJoint(LineSegment centerLine, double radius)
		//{
		//	articularSurface = new SymmetricCylinder(centerLine, radius);
		//}
		
		/// <summary>
		///     Returns the surface geometry used by this Hinge Joint.
		/// </summary>
		public override Surface GetArticularSurface()
		{
			return articularSurface;
		}
		
		/// <summary>
		///     Returns the raytraceable surface geometry used by this Hinge Joint.
		/// </summary>
		public override IExtendedRaytraceableSurface GetExtendedRaytraceableSurface()
		{
			return articularSurface;
		}
	}
}
