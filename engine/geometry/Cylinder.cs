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
using System.Linq;
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
		public Cylinder(Curve centerCurve, float radius)
		{
			this.CenterCurve = centerCurve;
			this.Radius = new ConstantFunction<Vector2, float>(radius);
			this.StartAngle = 0.0f;
			this.EndAngle = 2.0f * (float)Math.PI;
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
		/// <param name="centerLine">
		/// 	<inheritdoc cref="SymmetricCylinder.CenterCurve"/>
		/// </param>
		/// <param name="radius">
		/// 	<inheritdoc cref="SymmetricCylinder.Radius"/>
		/// </param>
		public Cylinder(Curve centerCurve, ContinuousMap<Vector2, float> radius)
		{
			this.CenterCurve = centerCurve;
			this.Radius = radius;
			this.StartAngle = 0.0f;
			this.EndAngle = 2.0f * (float)Math.PI;
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
		/// <param name="centerLine">
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
		public Cylinder(Curve centerCurve, ContinuousMap<Vector2, float> radius, ContinuousMap<float, float> startAngle, ContinuousMap<float, float> endAngle)
		{
			this.CenterCurve = centerCurve;
			this.Radius = radius;
			this.StartAngle = startAngle;
			this.EndAngle = endAngle;
		}
#endregion Constructors

#region Properties
		protected ContinuousMap<Vector2, float> radius2D;
		
		/// <summary>
		/// 	The radius of the cylinder at each point on the central axis. Mathematically, <c>radius</c> is a
		/// 	one-dimensional height function \f$f(t)\f$ defined on the domain \f$t \in [0, 1]\f$, where \f$t=0\f$ is
		/// 	the start point of the cylinder's central axis and \f$t=1\f$ is the end point of the cylinder's central
		/// 	axis.
		/// </summary>
		public ContinuousMap<Vector2, float> Radius
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
		/// <summary>
		public ContinuousMap<float, float> StartAngle { get; set; }
		
		/// <summary>
		/// 	The end angle of the pie-slice at each point on the central axis. <c>endAngle</c> is a
		/// 	one-dimensional function \f$\beta(t)\f$ defined on the domain \f$t \in [0, 1]\f$, where \f$t=0\f$ is
		/// 	the start point of the cylinder's central axis and \f$t=1\f$ is the end point of the cylinder's central
		/// 	axis. This allows one to vary the angles of the pie-slice along the length of the shaft.
		/// <summary>
		public ContinuousMap<float, float> EndAngle { get; set; }
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
		public override Vector3 GetNormalAt(Vector2 uv)
		{
			float u = uv.X;
			float v = uv.Y;
			
			Vector3 curveTangent = Vector3.Normalize(CenterCurve.GetTangentAt(v));
			Vector3 curveNormal = Vector3.Normalize(CenterCurve.GetNormalAt(v));
			Vector3 curveBinormal = Vector3.Cross(curveTangent, curveNormal);
			
			float startAngle = StartAngle.GetValueAt(v);
			float endAngle = EndAngle.GetValueAt(v);
			
			return (float)Math.Cos(u)*curveNormal + (float)Math.Sin(u)*curveBinormal;
		}
		
		/// <inheritdoc />
		public override Vector3 GetPositionAt(Vector2 uv)
		{
			float v = uv.Y;
				
			Vector3 translation = CenterCurve.GetPositionAt(v);
			float radius = Radius.GetValueAt(uv);
			
			return translation + radius*GetNormalAt(uv);
		}
		
		/// <inheritdoc />
		public override List<Vertex> GenerateVertexList(int resolutionU, int resolutionV)
		{
			var roughs = ParallelEnumerable.Range(0, resolutionV + 1).AsOrdered().SelectMany((j =>
			{
				float v = (float) j / (float) resolutionV;

				// Find the values at each ring:
				Vector3 curveTangent = Vector3.Normalize(CenterCurve.GetTangentAt(v));
				Vector3 curveNormal = Vector3.Normalize(CenterCurve.GetNormalAt(v));
				Vector3 curveBinormal = Vector3.Cross(curveTangent, curveNormal);

				Vector3 translation = CenterCurve.GetPositionAt(v);

				float startAngle = StartAngle.GetValueAt(v);
				float endAngle = EndAngle.GetValueAt(v);

				return Enumerable.Range(0, resolutionU).Select((i) =>
				{
					float u = startAngle + (endAngle - startAngle) * (float) i / (float) resolutionU;

					float radius = Radius.GetValueAt(new Vector2(u, v));

					// Calculate the position of the rings of vertices:
					Vector3 surfaceNormal = (float) Math.Cos(u) * curveNormal + (float) Math.Sin(u) * curveBinormal;
					Vector3 surfacePosition = translation + radius * surfaceNormal;
					return new Vertex(surfacePosition, surfaceNormal);
				});
			}));

			List<Vertex> output = roughs.ToList();

			// Recalculate the surface normal after deformation:
			for (int j = 1; j < resolutionV; j++)
			{
				for (int i = 0; i < (resolutionU - 1); i++)
				{
					Vector3 surfacePosition = output[(j-1)*resolutionU + i].Position;
					Vector3 du = surfacePosition - output[(j-1)*resolutionU + i + 1].Position;
					Vector3 dv = surfacePosition - output[(j)*resolutionU + i].Position;
					
					// Calculate the position of the rings of vertices:
					Vector3 surfaceNormal = Vector3.Cross(Vector3.Normalize(du), Vector3.Normalize(dv));
					
					output[(j-1)*resolutionU + i] = new Vertex(surfacePosition, surfaceNormal);
				}
				
				// Stitch the end of the triangles:
				Vector3 surfacePosition2 = output[(j-1)*resolutionU + resolutionU-1].Position;
				Vector3 du2 = surfacePosition2 - output[(j-1)*resolutionU].Position;
				Vector3 dv2 = surfacePosition2 - output[(j)*resolutionU + resolutionU-1].Position;
				
				// Calculate the position of the rings of vertices:
				Vector3 surfaceNormal2 = Vector3.Cross(Vector3.Normalize(du2), Vector3.Normalize(dv2));
				
				output[(j-1)*resolutionU + resolutionU-1] = new Vertex(surfacePosition2, surfaceNormal2);
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
