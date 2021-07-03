using System.Collections.Generic;
using System.Numerics;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public class SymmetricCylinder : Cylinder, IRaytraceableSurface
	{
		//public SymmetricCylinder(LineSegment centerLine, float radius)
		//	: base(centerLine, radius)
		//{
		//	radius1D = radius;
		//}
		
		public SymmetricCylinder(LineSegment centerLine, RaytraceableFunction1D radius)
			: base(centerLine, new DomainToVector2<float>(new Vector2(0.0f, 1.0f), radius))
		{
			radius1D = radius;
		}
		
		public SymmetricCylinder(LineSegment centerLine, RaytraceableFunction1D radius, ContinuousMap<float, float> startAngle, ContinuousMap<float, float> endAngle)
			: base(centerLine, new DomainToVector2<float>(new Vector2(0.0f, 1.0f), radius), startAngle, endAngle)
		{
			radius1D = radius;
		}
		
		protected RaytraceableFunction1D radius1D;
		public new RaytraceableFunction1D Radius
		{
			get { return radius1D; }
			set {
				radius1D = value;
				radius2D = new DomainToVector2<float>(new Vector2(0.0f, 1.0f), value);
			}
		}
		
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
			
			// TODO: ensure that CenterCurve is always a LineSegment!
			
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
				if ( phi > StartAngle.GetValueAt(t) && phi < EndAngle.GetValueAt(t) && i > 0.0f)
				{
					minimum = Math.Sign(i)*(float)Math.Min(Math.Abs(minimum), Math.Abs(i));
				}
			}
			
			return minimum;
		}
	}
}
