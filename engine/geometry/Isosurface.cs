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
using MathNet.Numerics.RootFinding;

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

		public double FindRootOnRay(Ray ray, int iterations)
		{
			// This function implements the Secant Rootfinding Method (see https://en.wikipedia.org/wiki/Secant_method):
			double x0 = 0.0;
			double x1 = 10.0;
			double value0;
			double value1 = (ScalarField(ray.StartPosition + x0 * ray.Direction) - Threshold);
			for (int i = 0; i < iterations; i++)
			{
				value0 = value1;
				value1 = (ScalarField(ray.StartPosition + x1 * ray.Direction) - Threshold);
				double x2 = x1 - (x1 - x0)/(value1 - value0);
				x0 = x1;
				x1 = x2;
			}
			return x1;
		}

		#region IRaytraceableSurface
		/// <inheritdoc />
		public RaySurfaceIntersection RayIntersect(Ray ray)
		{
			dvec3 rayStart = ray.StartPosition;
			dvec3 rayDirection = ray.Direction;
			DebugUtil.AssertAllFinite(rayStart, nameof(rayStart));
			DebugUtil.AssertAllFinite(rayDirection, nameof(rayDirection));

			double f(double x) => ScalarField(ray.StartPosition + x * ray.Direction) - Threshold;

			double distance = double.NaN;
			for (double x = 3.0; x>0.0; x -= 0.01)
			{
				double t = double.NaN;
				bool found = Bisection.TryFindRoot(f, x - 0.01, x, 10e-4, 100, out t);
				distance = found ? t : distance;
			}

			//Bisection.TryFindRoot(f, 0.0, 2.0, 10e-8, 500, out distance);


			//double distance = double.NaN;
			//Secant.TryFindRoot(f, 1.0, 10.0, 0.0, 20.0, 10e-6, 100, out distance);

			//double distance = FindRootOnRay(ray, 16);

			RaySurfaceIntersection minimum = new RaySurfaceIntersection(distance, 0.0, 0.0);
			
			return minimum;
		}
		#endregion IRaytraceableSurface

		#region IExtendedRaytraceableSurface
		/// <inheritdoc />
		public RayExtendedSurfaceIntersection ExtendedRayIntersect(Ray ray, double smoothingTypeValue)
		{
			RaySurfaceIntersection intersection = RayIntersect(ray);

			return new RayExtendedSurfaceIntersection(intersection.RayLength, 0.0);
		}
		#endregion IExtendedRaytraceableSurface
	}
}
