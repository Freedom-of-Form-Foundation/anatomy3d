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
using System;
using GlmSharp;

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
	public class SymmetricCylinder : Cylinder, IExtendedRaytraceableSurface
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
			: this(centerLine, radius, 0.0, 2.0 * Math.PI)
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
		public SymmetricCylinder(LineSegment centerLine, RaytraceableFunction1D radius, ContinuousMap<double, double> startAngle, ContinuousMap<double, double> endAngle)
			: base(centerLine, new DomainToVector2<double>(dvec2.UnitY, radius), startAngle, endAngle)
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
				radius2D = new DomainToVector2<double>(dvec2.UnitY, value);
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
		public RaySurfaceIntersection RayIntersect(Ray ray)
		{
			dvec3 rayStart = ray.StartPosition;
			dvec3 rayDirection = ray.Direction;
			DebugUtil.AssertAllFinite(rayStart, nameof(rayStart));
			DebugUtil.AssertAllFinite(rayDirection, nameof(rayDirection));
			
			// Since we raytrace only using a cylindrical surface that is horizontal and at the origin, we
			// first shift and rotate the ray such that we get the right orientation:
			dvec3 start = CenterCurve.GetStartPosition();
			dvec3 end = CenterCurve.GetEndPosition();
			DebugUtil.AssertAllFinite(start, nameof(start));
			DebugUtil.AssertAllFinite(end, nameof(end));
			
			dvec3 tangent = CenterCurve.GetTangentAt(0.0).Normalized;
			dvec3 normal = CenterCurve.GetNormalAt(0.0).Normalized;
			dvec3 binormal = CenterCurve.GetBinormalAt(0.0).Normalized;
			DebugUtil.AssertAllFinite(tangent, nameof(tangent));
			DebugUtil.AssertAllFinite(normal, nameof(normal));
			DebugUtil.AssertAllFinite(binormal, nameof(binormal));
			
			double length = dvec3.Distance(start, end);
			DebugUtil.AssertFinite(length, nameof(length));
			
			// CenterCurve is guaranteed to be a LineSegment, since the base property CenterCurve is masked by this
			// class' CenterCurve property that only accepts a LineSegment, and similarly this class' constructor only
			// accepts a LineSegment. The following mathematics, which assumes that the central axis is a line segment,
			// is therefore valid.

			dmat3 rotationMatrix = new dmat3(normal, binormal, tangent/length).Transposed;
			
			dvec3 rescaledRay = rotationMatrix * (rayStart - start);
			dvec3 newDirection = rotationMatrix * rayDirection.Normalized;
			

			double x0 = rescaledRay.x;
			double y0 = rescaledRay.y;
			double z0 = rescaledRay.z;
			
			double a = newDirection.x;
			double b = newDirection.y;
			double c = newDirection.z;
			
			// Raytrace using a cylindrical surface equation x^2 + y^2. The parameters in the following line
			// represent the coefficients of the expanded cylindrical surface equation, after the substitution
			// x = x_0 + a t and y = y_0 + b t:
			QuarticFunction surfaceFunction = new QuarticFunction(x0*x0 + y0*y0, 2.0*(x0*a + y0*b), a*a + b*b, 0.0, 0.0);
			
			IEnumerable<double> intersections = Radius.SolveRaytrace(surfaceFunction, z0, c);
			
			// The previous function returns a list of intersection distances. The value closest to 0.0f represents the
			// closest intersection point.

			RaySurfaceIntersection minimum = new RaySurfaceIntersection(Double.PositiveInfinity, 0.0, 0.0);

			foreach (double i in intersections)
			{
				// First calculate the parameter t on the surface of the SymmetricCylinder at which the ray intersects.
				double t = rescaledRay.z + i*newDirection.z;

				// t must be within the bounds of the SymmetricCylinder. t ranges from 0.0 to 1.0, so if it is outside
				// of this region, the ray does not intersect the cylinder.
				if (!(t >= 0.0 && t <= 1.0))
				{
					break;
				}

				// Find the angle to the normal of the centerLine, so that we can determine whether the
				// angle is within the bound of the pie-slice at position t:

				// Calculate the 3d point at which the ray intersects the cylinder:
				dvec3 intersectionPoint = rayStart + i * rayDirection;
				dvec3 centerLineNormal = CenterCurve.GetNormalAt(t);
				dvec3 centerLineBinormal = CenterCurve.GetBinormalAt(t);
				dvec3 d = intersectionPoint - CenterCurve.GetPositionAt(t);
				double correctionShift = Math.Sign(dvec3.Dot(d, centerLineBinormal));
				double phi = (correctionShift*Math.Acos(dvec3.Dot(d, centerLineNormal))) % (2.0*Math.PI);

				// Determine if the ray is inside the pie-slice of the cylinder that is being displayed,
				// otherwise discard:
				if ( phi > StartAngle.GetValueAt(t) && phi < EndAngle.GetValueAt(t) && i >= 0.0)
				{
					if (Math.Abs(i) < Math.Abs(minimum.RayLength))
					{
						minimum.RayLength = i;
						minimum.U = phi;
						minimum.V = t;
					}
				}
			}
			
			return minimum;
		}
		#endregion IRaytraceableSurface

		#region IExtendedRaytraceableSurface
		/// <inheritdoc />
		public RayExtendedSurfaceIntersection ExtendedRayIntersect(Ray ray)
		{
			RaySurfaceIntersection intersection = RayIntersect(ray);

			if(Double.IsNaN(intersection.RayLength) == false && Double.IsInfinity(intersection.RayLength) == false)
			{
				return new RayExtendedSurfaceIntersection(intersection.RayLength, 0.0);
			} else
			{
				dvec3 tangent = centerLine.GetTangentAt(0.0);
				LineSegment extendedCenterLine = new LineSegment(centerLine.GetStartPosition() - tangent, centerLine.GetEndPosition() + tangent);
				SortedList<double, double> radiusPoints = new SortedList<double, double>
				{
					{ 0.0, Radius.GetValueAt(0.0) },
					{ 1.0, Radius.GetValueAt(1.0) }
				};

				QuadraticSpline1D extendedRadius = new QuadraticSpline1D(radiusPoints);
				SymmetricCylinder extendedCylinder = new SymmetricCylinder(extendedCenterLine, extendedRadius);
				RaySurfaceIntersection extendedIntersection = extendedCylinder.RayIntersect(ray);

				double distanceFromBoundary = (Math.Abs(extendedIntersection.V * 3.0 - 1.5) - 1.0 / 2.0);
				distanceFromBoundary = (distanceFromBoundary > 0.0) ? distanceFromBoundary : 0.0;

				return new RayExtendedSurfaceIntersection(extendedIntersection.RayLength, distanceFromBoundary);
			}
		}
		#endregion IExtendedRaytraceableSurface
	}
}
