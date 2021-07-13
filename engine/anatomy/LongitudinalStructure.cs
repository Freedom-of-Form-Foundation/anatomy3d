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


/// Please pardon the "Cargo cult" programming ^.-.^;;;
using System.Collections.Generic;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Anatomy;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Anatomy
{
	/// <summary>
	/// A <c>LongitudinalStructure</c> represents the simplest geometry defining a first-order approximation
	/// of long anatomical shapes, such as a Long Bone. LongitudinalStructure includes information of the central
	/// spline (which might be a single spline, or branching), as well as diameter or other information about 
	/// cross-sectional profile along that spline's shape. In this way, the simple first-order approximation can
	/// be separated from more unique and interesting properties of bones, joints, muscles, and other anatomical parts.
	/// </summary>
	public class LongitudinalStructure
	{
		#region Constructors
		/// <summary>
		/// 	Construct a new <c>LongitudinalStructure</c> around a central curve, the <c>centerCurve</c>.
		/// 	The radius at each point on the central axis is defined by a two-dimensional heightmap function
		/// 	<c>radius</c>.
		/// </summary>
		/// <param name="centerCurve">
		/// 	<inheritdoc cref="LongitudinalStructure.CenterCurve"/>
		/// </param>
		/// <param name="radius">
		/// 	<inheritdoc cref="LongitudinalStructure.Radius"/>
		/// </param>
		public LongitudinalStructure(Curve centerCurve, ContinuousMap<Vector2, float> radius)
		{
			this.CenterCurve = centerCurve;
			this.Radius = radius;
		}


#endregion Constructors

#region Properties


		/// <summary>
		/// The surface geometry of the LongitudinalStructure, as calculated from the spline and radial map.
		/// </summary>
		private Capsule _basicShell;


		/// <summary>
		/// A flag used for tracking whether vertex data needs to be redrawn or not.
		/// </summary>
		private bool _MUST_REDRAW = true;

		private ContinuousMap<Vector2, float> _radius;
		private Curve _centerCurve;

		/// <summary>
		/// The two-dimensional height map defining the radius of the <c>Capsule</c> that represents the LongitudinalStructure.
		/// 
		/// <see cref="FreedomOfFormFoundation.AnatomyEngine.Geometry.Capsule"/> for more information on the
		/// properties and domain of this height map.
		/// </summary>
		public ContinuousMap<Vector2, float> Radius {
			get
			{
				return _radius;
			}
			set
			{
				_radius = value;
				_MUST_REDRAW = true;
			}
		}

		/// <summary>
		/// The central curve along the length of the shaft of the LongitudinalStructure around which the <c>Capsule</c> is
		/// generated.
		/// </summary>
		public Curve CenterCurve { 
			get
			{
				return _centerCurve;
			}
			set
			{
				_centerCurve = value;
				_MUST_REDRAW = true;
			}
		}
#endregion Properties

#region Renderable utilization of the class
		/// <summary>
		/// Returns the surface geometry of the LongitudinalStructure after all constraints and deformations up to this point
		/// have been taken into account.
		/// </summary>
		public Surface GetGeometry()
		{
			if (_MUST_REDRAW == true)
			{
				_basicShell = new Capsule(_centerCurve, _radius);
				_MUST_REDRAW = false;
			}

			return _basicShell;

		}
#endregion Renderable utilization of the class
	}
}
