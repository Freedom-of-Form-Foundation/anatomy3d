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
using System;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	/// <summary>
	/// 	A <c>SymmetricCylinder</c> represents a cylindrical surface that can have a varying radius along
	/// 	its length. Unlike a <c>Cylinder</c>, however, its central axis cannot be bent and must be a straight line,
	/// 	and the radius must be radially constant, i.e. the radius can only change along the length of the cylinder,
	/// 	but not along the radial axis. These properties make this surface perfect to represent a hinge joint
	/// 	surface. Due to its simplicity, it is raytraceable.
	/// </summary>
	public class SymmetricCylinder : Cylinder, IRaytraceableSurface
	{
#region Constructors
		/// <summary>
		/// 	Construct a new <c>SymmetricCylinder</c> around a central axis, the <c>centerLine</c>. The radius at
		/// 	each point on the central axis is defined by a one-dimensional function <c>radius</c>.
		/// 	
		/// 	The surface is parametrized with the coordinates \f$t \in [0, 1]\f$ (along the length of the shaft) and
		/// 	\f$\phi \in [0, 2 \pi]\f$ (along the radial coordinate).
		/// </summary>
		/// <param name="centerLine">
		/// 	<inheritdoc cref="SymmetricCylinder.CenterCurve"/>
		/// </param>
		/// <param name="radius">
		/// 	<inheritdoc cref="SymmetricCylinder.Radius"/>
		/// </param>
		public SymmetricCylinder(LineSegment centerLine, RaytraceableFunction1D radius)
			: this(centerLine, radius, 0.0f, 2.0f*(float)Math.PI)
		{
			
		}
		
		/// <summary>
		/// 	Construct a pie-slice of a new <c>SymmetricCylinder</c> around a central axis, the <c>centerLine</c>.
		/// 	The radius at each point on the central axis is defined by a one-dimensional function <c>radius</c>.
		/// 	The cylinder is cut in a pie-slice along the length of the shaft, defined by the angles
		/// 	<c>startAngle</c> and <c>endAngle</c>.
		/// 	
		/// 	The surface is parametrized with the coordinates \f$t \in [0, 1]\f$ (along the length of the shaft) and
		/// 	\f$\phi \in [0, 2 \pi]\f$ (along the radial coordinate).
		/// 	
		/// 	This gives a pie-sliced cylindrical surface that is defined only between the angles <c>startAngle</c>
		/// 	and <c>endAngle</c>. For example, if <c>startAngle = 0.0f</c> and <c>endAngle = Math.PI</c>, one would
		/// 	get a cylinder that is sliced in half.
		/// </summary>
		/// <param name="centerLine">
		/// 	<inheritdoc cref="SymmetricCylinder.CenterCurve"/>
		/// </param>
		/// <param name="radius">
		/// 	<inheritdoc cref="SymmetricCylinder.Radius"/>
		/// </param>
		/// <param name="startAngle">
		/// 	The starting angle of the pie-slice at each point on the central axis. <c>startAngle</c> is a
		/// 	one-dimensional function \f$\alpha(t)\f$ defined on the domain \f$t \in [0, 1]\f$, where \f$t=0\f$ is
		/// 	the start point of the cylinder's central axis and \f$t=1\f$ is the end point of the cylinder's central
		/// 	axis. This allows one to vary the angles of the pie-slice along the length of the shaft.
		/// </param>
		/// <param name="endAngle">
		/// 	The end angle of the pie-slice at each point on the central axis. <c>endAngle</c> is a
		/// 	one-dimensional function \f$\beta(t)\f$ defined on the domain \f$t \in [0, 1]\f$, where \f$t=0\f$ is
		/// 	the start point of the cylinder's central axis and \f$t=1\f$ is the end point of the cylinder's central
		/// 	axis. This allows one to vary the angles of the pie-slice along the length of the shaft.
		/// </param>
		public SymmetricCylinder(LineSegment centerLine, RaytraceableFunction1D radius, ContinuousMap<float, float> startAngle, ContinuousMap<float, float> endAngle)
			: base(centerLine, new DomainToVector2<float>(new Vector2(0.0f, 1.0f), radius), startAngle, endAngle)
		{
			radius1D = radius;
			this.centerLine = centerLine;
		}
#endregion

#region Properties
		protected RaytraceableFunction1D radius1D;
		
		/// <summary>
		/// 	<inheritdoc cref="Cylinder.Radius"/>
		/// </summary>
		public new RaytraceableFunction1D Radius
		{
			get { return radius1D; }
			set {
				radius1D = value;
				radius2D = new DomainToVector2<float>(new Vector2(0.0f, 1.0f), value);
			}
		}
		
		protected LineSegment centerLine;
		
		/// <summary>
		/// 	The line segment that defines the central axis of the cylinder.
		/// </summary>
		public new LineSegment CenterCurve
		{
			get { return centerLine; }
			set {
				centerLine = value;
				base.CenterCurve = value;
			}
		}
#endregion Properties

#region IRaytraceableSurface
		/// <inheritdoc />
		public float RayIntersect(Ray ray)
		{
			Vector3 rayStart = ray.StartPosition;
			Vector3 rayDirection = ray.Direction;
			
			// Since we raytrace only using a cylindrical surface that is horizontal and at the origin, we
			// first shift and rotate the ray such that we get the right orientation:
			Vector3 start = CenterCurve.GetStartPosition();
			Vector3 end = CenterCurve.GetEndPosition();
			
			Vector3 tangent = Vector3.Normalize(CenterCurve.GetTangentAt(0.0f));
			Vector3 normal = Vector3.Normalize(CenterCurve.GetNormalAt(0.0f));
			Vector3 binormal = Vector3.Normalize(CenterCurve.GetBinormalAt(0.0f));
			
			float length = Vector3.Distance(start, end);
			
			// CenterCurve is guaranteed to be a LineSegment, since the base property CenterCurve is masked by this
			// class' CenterCurve property that only accepts a LineSegment, and similarly this class' constructor only
			// accepts a LineSegment. The following mathematics, which assumes that the central axis is a line segment,
			// is therefore valid.
			
			Matrix4x4 rotationMatrix = new Matrix4x4(normal.X, binormal.X, tangent.X/length, 0.0f,
			                                         normal.Y, binormal.Y, tangent.Y/length, 0.0f,
			                                         normal.Z, binormal.Z, tangent.Z/length, 0.0f,
			                                         0.0f, 0.0f,   0.0f,  1.0f);
			
			Vector3 rescaledRay = Vector3.Transform(rayStart - start, rotationMatrix);
			Vector3 newDirection = Vector3.TransformNormal(Vector3.Normalize(rayDirection), rotationMatrix);
			
			
			float x0 = rescaledRay.X;
			float y0 = rescaledRay.Y;
			float z0 = rescaledRay.Z;
			
			float a = newDirection.X;
			float b = newDirection.Y;
			float c = newDirection.Z;
			
			// Raytrace using a cylindrical surface equation x^2 + y^2. The parameters in the following line
			// represent the coefficients of the expanded cylindrical surface equation, after the substitution
			// x = x_0 + a t and y = y_0 + b t:
			QuarticFunction surfaceFunction = new QuarticFunction(x0*x0 + y0*y0, 2.0f*(x0*a + y0*b), a*a + b*b, 0.0f, 0.0f);
			
			IEnumerable<float> intersections = Radius.SolveRaytrace(surfaceFunction, z0, c);
			
			// The previous function returns a list of intersection distances. The value closest to 0.0f represents the
			// closest intersection point.
			float minimum = Single.PositiveInfinity;
			
			foreach (float i in intersections)
			{
				// Calculate the 3d point at which the ray intersects the cylinder:
				Vector3 intersectionPoint = rayStart + i*rayDirection;
				
				// Find the closest point to the intersectionPoint on the centerLine.
				// Get the vector v from the start of the cylinder to the intersection point:
				Vector3 v = intersectionPoint - start;
				
				// ...And project this vector onto the center line:
				float t = -Vector3.Dot(intersectionPoint, tangent*length)/(length*length);
				
				// Now we have the parameter t on the surface of the SymmetricCylinder at which the ray intersects.
				
				// Find the angle to the normal of the centerLine, so that we can determine whether the
				// angle is within the bound of the pie-slice at position t:
				Vector3 centerLineNormal = CenterCurve.GetNormalAt(t);
				Vector3 centerLineBinormal = CenterCurve.GetBinormalAt(t);
				Vector3 d = intersectionPoint - CenterCurve.GetPositionAt(t);
				float correctionShift = (float)Math.Sign(Vector3.Dot(d, centerLineBinormal));
				float phi = (correctionShift*(float)Math.Acos(Vector3.Dot(d, centerLineNormal))) % (2.0f*(float)Math.PI);
				
				// Determine if the ray is inside the pie-slice of the cylinder that is being displayed,
				// otherwise discard:
				if ( phi > StartAngle.GetValueAt(t) && phi < EndAngle.GetValueAt(t) && i >= 0.0f)
				{
					minimum = Math.Sign(i)*(float)Math.Min(Math.Abs(minimum), Math.Abs(i));
				}
			}
			
			return minimum;
		}
#endregion IRaytraceableSurface
	}
}
