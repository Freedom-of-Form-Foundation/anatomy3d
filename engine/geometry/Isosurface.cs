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
	/// 	An <c>Isosurface</c> represents a raytraceable surface that is formed by all points where some 3D scalar
	/// 	field is a constant value, i.e. \f$f(\vec{v}) = a\f$. Although it is raytraceable, it can be hard to
	/// 	turn into a mesh. Therefore, it is currently not mesh-renderable and instead is used by other surfaces for
	/// 	raytracing only.
	/// </summary>
	public class Isosurface : IExtendedRaytraceableSurface
	{
		#region Constructors
		/// <summary>
		/// 	Construct a new <c>Isosurface</c>. The surface is not parametrized.
		/// </summary>
		/// <param name="scalarField">
		/// 	The scalar field from which to sample the isosurface. It does not need to be continuous.
		/// </param>
		public Isosurface(Func<dvec3, double> scalarField, double threshold)
		{
			ScalarField = scalarField;
			Threshold = threshold;
		}
		#endregion

		#region Properties
		public Func<dvec3, double> ScalarField { get; set; }
		public double Threshold { get; set; }
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

			if (Double.IsNaN(intersection.RayLength) == false && Double.IsInfinity(intersection.RayLength) == false)
			{
				return new RayExtendedSurfaceIntersection(intersection.RayLength, 0.0);
			}

			dvec3 tangent = centerLine.GetTangentAt(0.0);

			dvec3 curveNormal = CenterCurve.GetNormalAt(0.0);
			dvec3 curveBinormal = CenterCurve.GetBinormalAt(0.0);

			dvec3 planeOrigin1 = GetPositionAt(new dvec2(StartAngle.GetValueAt(0.0), 0.0));
			double phi1 = StartAngle.GetValueAt(0.0);
			dvec3 vectorV1 = Math.Sin(phi1) * curveNormal - Math.Cos(phi1) * curveBinormal;
			CorrugatedPlane extendedPlane1 = new CorrugatedPlane(CenterCurve.GetStartPosition(), tangent, vectorV1, Radius, 1.0);

			RaySurfaceIntersection extendedPlaneIntersection1 = extendedPlane1.RayIntersect(ray);

			double distanceFromBoundary1 = extendedPlaneIntersection1.V;

			if (Double.IsNaN(extendedPlaneIntersection1.RayLength) == false && Double.IsInfinity(extendedPlaneIntersection1.RayLength) == false)
			{
				return new RayExtendedSurfaceIntersection(extendedPlaneIntersection1.RayLength, distanceFromBoundary1);
			}

			dvec3 planeOrigin2 = GetPositionAt(new dvec2(EndAngle.GetValueAt(0.0), 0.0));
			double phi2 = EndAngle.GetValueAt(0.0);
			dvec3 vectorV2 = -Math.Sin(phi2) * curveNormal + Math.Cos(phi2) * curveBinormal;
			CorrugatedPlane extendedPlane2 = new CorrugatedPlane(CenterCurve.GetStartPosition(), tangent, vectorV2, Radius, -1.0);

			RaySurfaceIntersection extendedPlaneIntersection2 = extendedPlane2.RayIntersect(ray);

			double distanceFromBoundary2 = extendedPlaneIntersection2.V;

			if (Double.IsNaN(extendedPlaneIntersection2.RayLength) == false && Double.IsInfinity(extendedPlaneIntersection2.RayLength) == false)
			{
				return new RayExtendedSurfaceIntersection(extendedPlaneIntersection2.RayLength, distanceFromBoundary2);
			}

			LineSegment extendedCenterLine = new LineSegment(centerLine.GetStartPosition() - tangent, centerLine.GetEndPosition() + tangent);
			SortedList<double, double> radiusPoints = new SortedList<double, double>
						{
							{ 0.0, 0.0 },
							{ 0.05, 0.05 },
							{ 1.0/3.0, Radius.GetValueAt(0.0) },
							{ 2.0/3.0, Radius.GetValueAt(1.0) },
							{ 0.95, 0.05 },
							{ 1.0, 0.0 }
						};

			QuadraticSpline1D extendedRadius = new QuadraticSpline1D(radiusPoints);
			SymmetricCylinder extendedCylinder = new SymmetricCylinder(extendedCenterLine, extendedRadius);
			RaySurfaceIntersection extendedIntersection = extendedCylinder.RayIntersect(ray);

			double distanceFromBoundary = (Math.Abs(extendedIntersection.V * 3.0 - 1.5) - 1.0 / 2.0);
			distanceFromBoundary = (distanceFromBoundary > 0.0) ? distanceFromBoundary : 0.0;

			return new RayExtendedSurfaceIntersection(extendedIntersection.RayLength, distanceFromBoundary);
		}
		#endregion IExtendedRaytraceableSurface
	}
}
