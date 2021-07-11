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
using FreedomOfFormFoundation.AnatomyEngine.Anatomy;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Anatomy.Bones
{
	/// <summary>
	/// A <c>LongBone</c> represents the anatomical shape of a 'long bone', such as a humerus. Long bones are bones
	/// classified to have a relatively long length compared to their diameter, and are approximated in the engine
	/// as a deformed <c>Capsule</c>.
	/// </summary>
	public class LongBone : Bone
	{
#region Constructors
		/// <summary>
		/// 	Construct a new <c>LongBone</c> around a central curve, the <c>centerCurve</c>.
		/// 	The radius at each point on the central axis is defined by a two-dimensional heightmap function
		/// 	<c>radius</c>.
		/// </summary>
		/// <param name="centerCurve">
		/// 	<inheritdoc cref="LongBone.CenterCurve"/>
		/// </param>
		/// <param name="radius">
		/// 	<inheritdoc cref="LongBone.Radius"/>
		/// </param>
		public LongBone(Curve centerCurve, ContinuousMap<Vector2, float> radius)
		{
			this.CenterCurve = centerCurve;
			this.Radius = radius;
			this.InteractingJoints = new List<JointDeformation>(0);
		}
		
		/// <summary>
		/// 	Construct a new <c>LongBone</c> around a central curve, the <c>centerCurve</c>.
		/// 	The radius at each point on the central axis is defined by a one-dimensional heightmap function
		/// 	<c>radius</c>, only changing the radius along the length of the bone but retaining radial symmetry.
		/// </summary>
		/// <param name="centerCurve">
		/// 	<inheritdoc cref="LongBone.CenterCurve"/>
		/// </param>
		/// <param name="radius">
		/// 	<inheritdoc cref="LongBone.Radius"/>
		/// </param>
		public LongBone(Curve centerCurve, ContinuousMap<float, float> radius)
			: this(centerCurve, new DomainToVector2<float>(new Vector2(0.0f, 1.0f), radius))
		{
			
		}
		
		/// <summary>
		/// 	Construct a new <c>LongBone</c> around a central curve, the <c>centerCurve</c>.
		/// 	The radius at each point on the central axis is defined by a constant <c>radius</c>.
		/// </summary>
		/// <param name="centerCurve">
		/// 	<inheritdoc cref="LongBone.CenterCurve"/>
		/// </param>
		/// <param name="radius">
		/// 	The constant radius of the <c>Capsule</c> representing the shape of the bone.
		/// </param>
		public LongBone(Curve centerCurve, float radius)
			: this(centerCurve, new ConstantFunction<Vector2, float>(radius))
		{
			
		}
#endregion Constructors

#region Properties
		/// <summary>
		/// The two-dimensional height map defining the radius of the <c>Capsule</c> that represents the long bone.
		/// 
		/// <see cref="FreedomOfFormFoundation.AnatomyEngine.Geometry.Capsule"/> for more information on the
		/// properties and domain of this height map.
		/// </summary>
		public ContinuousMap<Vector2, float> Radius { get; set; }
		
		/// <summary>
		/// The central curve along the length of the shaft of the long bone around which the <c>Capsule</c> is
		/// generated.
		/// </summary>
		public Curve CenterCurve { get; set; }
		
		/// <summary>
		/// The list of joints that the bone interacts with. The bone is sequentially deformed by the entries of
		/// this list to ensure that the bone correctly fits inside each joint geometry.
		/// </summary>
		public List<JointDeformation> InteractingJoints { get; set; }
#endregion Properties

#region Base Class Method Overrides
		/// <summary>
		/// Returns the surface geometry of the bone after all constraints and deformations up to this point
		/// have been taken into account.
		/// </summary>
		public override Surface GetGeometry()
		{
			// Sequentially add the influence of each joint to the bone's radial deformations:
			ContinuousMap<Vector2, float> deformations = Radius;
			foreach (var i in InteractingJoints)
			{
				MoldCastMap boneHeightMap = new MoldCastMap(CenterCurve,
				                                            i.InteractingJoint.GetRaytraceableSurface(),
				                                            deformations,
				                                            i.Direction,
				                                            i.MaxDistance);
				deformations = boneHeightMap;
			}
			
			return new Capsule(CenterCurve, deformations);
		}
#endregion Base Class Method Overrides
	}
}
