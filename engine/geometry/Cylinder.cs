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
using System.Linq;
using GlmSharp;

using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	/// <summary>
	/// 	A <c>Cylinder</c> represents a (bent) cylindrical surface around a <c>Curve</c> that can have a varying
	/// 	radius along its length.
	/// </summary>
	public class Cylinder : Surface
	{
#region Constructors
		/// <summary>
		/// 	Construct a new <c>Cylinder</c> around a central curve, the <c>centerCurve</c>.
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
		/// <param name="centerCurve">
		/// 	<inheritdoc cref="Cylinder.CenterCurve"/>
		/// </param>
		/// <param name="radius">
		/// 	<inheritdoc cref="Cylinder.Radius"/>
		/// </param>
		public Cylinder(Curve centerCurve, double radius)
		{
			DebugUtil.AssertFinite(radius, nameof(radius));
			this.CenterCurve = centerCurve;
			radius2D = new ConstantFunction<dvec2, double>(radius);
			this.StartAngle = 0.0;
			this.EndAngle = 2.0 * Math.PI;
		}
		
		/// <summary>
		/// 	Construct a pie-slice of a new <c>Cylinder</c> around a central axis, the <c>centerLine</c>.
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
		/// <param name="centerCurve">
		/// 	<inheritdoc cref="SymmetricCylinder.CenterCurve"/>
		/// </param>
		/// <param name="radius">
		/// 	<inheritdoc cref="SymmetricCylinder.Radius"/>
		/// </param>
		public Cylinder(Curve centerCurve, ContinuousMap<dvec2, double> radius)
		{
			this.CenterCurve = centerCurve;
			radius2D = radius;
			this.StartAngle = 0.0;
			this.EndAngle = 2.0 * Math.PI;
		}
		
		/// <summary>
		/// 	Construct a pie-slice of a new <c>Cylinder</c> around a central axis, the <c>centerLine</c>.
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
		/// <param name="centerCurve">
		/// 	<inheritdoc cref="Cylinder.CenterCurve"/>
		/// </param>
		/// <param name="radius">
		/// 	<inheritdoc cref="Cylinder.Radius"/>
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
		public Cylinder(Curve centerCurve, ContinuousMap<dvec2, double> radius, ContinuousMap<double, double> startAngle, ContinuousMap<double, double> endAngle)
		{
			this.CenterCurve = centerCurve;
			radius2D = radius;
			this.StartAngle = startAngle;
			this.EndAngle = endAngle;
		}
#endregion Constructors

#region Properties
		protected ContinuousMap<dvec2, double> radius2D;
		
		/// <summary>
		/// 	The radius of the cylinder at each point on the central axis. Mathematically, <c>radius</c> is a
		/// 	one-dimensional height function \f$f(t)\f$ defined on the domain \f$t \in [0, 1]\f$, where \f$t=0\f$ is
		/// 	the start point of the cylinder's central axis and \f$t=1\f$ is the end point of the cylinder's central
		/// 	axis.
		/// </summary>
		public ContinuousMap<dvec2, double> Radius
		{
			get { return radius2D; }
			set { radius2D = value; }
		}
		
		/// <summary>
		/// 	The curve around which the cylinder is generated.
		/// </summary>
		public Curve CenterCurve { get; set; }
		
		/// <summary>
		/// 	The starting angle of the pie-slice at each point on the central axis. <c>startAngle</c> is a
		/// 	one-dimensional function \f$\alpha(t)\f$ defined on the domain \f$t \in [0, 1]\f$, where \f$t=0\f$ is
		/// 	the start point of the cylinder's central axis and \f$t=1\f$ is the end point of the cylinder's central
		/// 	axis. This allows one to vary the angles of the pie-slice along the length of the shaft.
		/// </summary>
		public ContinuousMap<double, double> StartAngle { get; set; }
		
		/// <summary>
		/// 	The end angle of the pie-slice at each point on the central axis. <c>endAngle</c> is a
		/// 	one-dimensional function \f$\beta(t)\f$ defined on the domain \f$t \in [0, 1]\f$, where \f$t=0\f$ is
		/// 	the start point of the cylinder's central axis and \f$t=1\f$ is the end point of the cylinder's central
		/// 	axis. This allows one to vary the angles of the pie-slice along the length of the shaft.
		/// </summary>
		public ContinuousMap<double, double> EndAngle { get; set; }
#endregion Properties

#region Static Methods
		/// <summary>
		/// 	Returns the amount of vertices that the hemisphere would have at a certain resolution.
		/// </summary>
		public static int CalculateVertexCount(int resolutionU, int resolutionV)
		{
			return resolutionU * (resolutionV + 1);
		}
		
		/// <summary>
		/// 	Returns the amount of indices that the hemisphere would have at a certain resolution.
		/// </summary>
		public static int CalculateIndexCount(int resolutionU, int resolutionV)
		{
			return resolutionU * resolutionV * 6;
		}
#endregion Static Methods

#region Base Class Method Overrides
		/// <inheritdoc />
		public override dvec3 GetNormalAt(dvec2 uv)
		{
			DebugUtil.AssertAllFinite(uv, nameof(uv));
			double u = uv.x;
			double v = uv.y;
			
			dvec3 curveTangent = CenterCurve.GetTangentAt(v).Normalized;
			dvec3 curveNormal = CenterCurve.GetNormalAt(v).Normalized;
			dvec3 curveBinormal = dvec3.Cross(curveTangent, curveNormal);

			/*
			double startAngle = StartAngle.GetValueAt(v);
			double endAngle = EndAngle.GetValueAt(v);
			*/

			return Math.Cos(u)*curveNormal + Math.Sin(u)*curveBinormal;
		}
		
		/// <inheritdoc />
		public override dvec3 GetPositionAt(dvec2 uv)
		{
			DebugUtil.AssertAllFinite(uv, nameof(uv));
			double v = uv.y;
				
			dvec3 translation = CenterCurve.GetPositionAt(v);
			double radius = Radius.GetValueAt(uv);
			
			return translation + radius*GetNormalAt(uv);
		}
		
		/// <inheritdoc />
		public override List<Vertex> GenerateVertexList(int resolutionU, int resolutionV)
		{
			// If asked to sample at zero points, return an empty list.
			if (resolutionU <= 0 || resolutionV <= 0)
			{
				return new List<Vertex>();
			}
			var roughs = ParallelEnumerable.Range(0, resolutionV + 1).AsOrdered().SelectMany((j =>
			{
				double v = j / (double) resolutionV;

				// Find the values at each ring:
				dvec3 curveTangent = CenterCurve.GetTangentAt(v).Normalized;
				dvec3 curveNormal = CenterCurve.GetNormalAt(v).Normalized;
				dvec3 curveBinormal = dvec3.Cross(curveTangent, curveNormal);

				dvec3 translation = CenterCurve.GetPositionAt(v);

				double startAngle = StartAngle.GetValueAt(v);
				double endAngle = EndAngle.GetValueAt(v);

				return Enumerable.Range(0, resolutionU).Select((i) =>
				{
					double u = startAngle + (endAngle - startAngle) * i / resolutionU;

					double radius = Radius.GetValueAt(new dvec2(u, v));

					// Calculate the position of the rings of vertices:
					dvec3 surfaceNormal = Math.Cos(u) * curveNormal + Math.Sin(u) * curveBinormal;
					dvec3 surfacePosition = translation + radius * surfaceNormal;
					return new Vertex((vec3)surfacePosition, (vec3)surfaceNormal);
				});
			}));

			List<Vertex> output = roughs.ToList();

			// Recalculate the surface normal after deformation:
			for (int j = 1; j < resolutionV; j++)
			{
				for (int i = 0; i < (resolutionU - 1); i++)
				{
					dvec3 surfacePosition = output[(j-1)*resolutionU + i].Position;
					dvec3 du = surfacePosition - output[(j-1)*resolutionU + i + 1].Position;
					dvec3 dv = surfacePosition - output[(j)*resolutionU + i].Position;
					
					// Calculate the position of the rings of vertices:
					dvec3 surfaceNormal = dvec3.Cross(du.Normalized, dv.Normalized);
					
					output[(j-1)*resolutionU + i] = new Vertex((vec3)surfacePosition, (vec3)surfaceNormal);
				}
				
				// Stitch the end of the triangles:
				dvec3 surfacePosition2 = output[(j-1)*resolutionU + resolutionU-1].Position;
				dvec3 du2 = surfacePosition2 - output[(j-1)*resolutionU].Position;
				dvec3 dv2 = surfacePosition2 - output[(j)*resolutionU + resolutionU-1].Position;
				
				// Calculate the position of the rings of vertices:
				dvec3 surfaceNormal2 = dvec3.Cross(du2.Normalized, dv2.Normalized);
				
				output[(j-1)*resolutionU + resolutionU-1] = new Vertex((vec3)surfacePosition2, (vec3)surfaceNormal2);
			}
			
			return output;
		}
		
		/// <inheritdoc />
		public override List<int> GenerateIndexList(int resolutionU, int resolutionV, int indexOffset = 0)
		{
			List<int> output = new List<int>(CalculateIndexCount(resolutionU, resolutionV));
			
			// Add the remaining rings:
			for (int j = 0; j < resolutionV; j++)
			{
				// Add a ring of triangles:
				for (int i = 0; i < resolutionU-1; i++)
				{
					output.Add(indexOffset + i + resolutionU*j);
					output.Add(indexOffset + i + resolutionU*(j+1));
					output.Add(indexOffset + (i+1) + resolutionU*j);

					output.Add(indexOffset + i + resolutionU*(j+1));
					output.Add(indexOffset + (i+1) + resolutionU*(j+1));
					output.Add(indexOffset + (i+1) + resolutionU*j);
				}
				
				// Stitch the end of the triangles:
				output.Add(indexOffset + resolutionU-1 + resolutionU*j);
				output.Add(indexOffset + resolutionU-1 + resolutionU*(j+1));
				output.Add(indexOffset + resolutionU*j);

				output.Add(indexOffset + resolutionU-1 + resolutionU*(j+1));
				output.Add(indexOffset + resolutionU*(j+1));
				output.Add(indexOffset + resolutionU*j);
			}

			return output;
		}
#endregion Base Class Method Overrides
	}
}
