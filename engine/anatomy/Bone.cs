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
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Anatomy
{
	/// <summary>
	/// A <c>Bone</c> represents an anatomical bone shape.
	/// </summary>
	public abstract class Bone : BodyPart
	{
		/// <summary>
		/// A <c>Bone.JointDeformation</c> defines the shape influence that a joint object has on a bone. For example,
		/// in the humerus, a hinge joint at the elbow region defines the shape of the humerus, and the humerus must
		/// be deformed according to the shape of the joint. In the engine, this is done by raycasting from the surface
		/// of the bone to the joint, and treating the joint as a mold for the bone to be cast into. This struct defines
		/// how the joint is meant to deform the bone, such as in which direction rays are cast, or smoothing properties
		/// (to be added to the code in the future).
		/// </summary>
		public struct JointDeformation
		{
			/// <summary>
			/// The joint object that the bone must reshape itself to.
			/// </summary>
			public Joint InteractingJoint { get; set; }
			
			/// <summary>
			/// Whether the rays are cast inwardly or outwardly.
			/// </summary>
			public RayCastDirection Direction { get; set; }

			/// <summary>
			/// Whether the rays are cast inwardly or outwardly.
			/// </summary>
			public double SmoothingTypeValue { get; set; }

			/// <summary>
			/// The maximal raycast distance before a ray is discarded; i.e. the maximal distance of the joint's surface
			/// from the bone's surface. This prevents joints from altering the shape of distant regions of a bone that
			/// happen to be pointing in the direction of the joint, but aren't actually part of the physiological joint
			/// region.
			/// </summary>
			public float MaxDistance { get; set; }
			
			public JointDeformation(Joint interactingJoint, RayCastDirection direction, float maxDistance, double smoothingTypeValue)
			{
				InteractingJoint = interactingJoint;
				Direction = direction;
				MaxDistance = maxDistance;
				SmoothingTypeValue = smoothingTypeValue;
			}
		}
		
		/// <summary>
		/// Returns the surface geometry used by this Bone.
		/// </summary>
		public override Surface GetGeometry()
		{
			return new Hemisphere(1.0, new dvec3(0.0, 0.0, 0.0), new dvec3(0.0, 1.0, 0.0));
		}
	}
}
