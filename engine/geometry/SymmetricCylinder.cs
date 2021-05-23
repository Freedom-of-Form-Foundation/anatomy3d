using System.Collections.Generic;
using System.Numerics;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public class SymmetricCylinder : Cylinder, IRaytraceableSurface
	{
		public SymmetricCylinder(Line centerLine, float radius)
			: base(centerLine, radius)
		{
			
		}
		
		public SymmetricCylinder(Line centerLine, ContinuousMap<float, float> radius)
			: base(centerLine, new DomainToVector2<float>(new Vector2(0.0f, 1.0f), radius))
		{
			
		}
		
		public SymmetricCylinder(Line centerLine, ContinuousMap<float, float> radius, ContinuousMap<float, float> startAngle, ContinuousMap<float, float> endAngle)
			: base(centerLine, new DomainToVector2<float>(new Vector2(0.0f, 1.0f), radius), startAngle, endAngle)
		{
			
		}
		
		public float RayIntersect(Vector3 rayStart, Vector3 rayDirection)
		{
			// Since we raytrace only using a cylindrical surface that is horizontal and at the origin, we
			// first shift and rotate the ray such that we get the right orientation:
			Vector3 start = CenterCurve.GetStartPosition();
			Vector3 end = CenterCurve.GetEndPosition();
			float length = Vector3.Distance(start, end);
			
			Vector3 shiftedRay = rayStart - start;
			Vector3 rescaledRay = new Vector3(shiftedRay.X, shiftedRay.Y, shiftedRay.Z/length);
			
			float x0 = rescaledRay.X;
			float y0 = rescaledRay.Y;
			float z0 = rescaledRay.Z;
			
			Vector3 newDirection = new Vector3(rayDirection.X, rayDirection.Y, rayDirection.Z/length);
			
			float a = newDirection.X;
			float b = newDirection.Y;
			float c = newDirection.Z;
			
			// TODO: ensure that CenterCurve is always a Line!
			
			// Raytrace using a cylindrical surface equation x^2 + y^2. The parameters in the following line
			// represent the coefficients of the expanded cylindrical surface equation, after the substitution
			// x = x_0 + a t and y = y_0 + b t:
			QuarticFunction surfaceFunction = new QuarticFunction(x0*x0 + y0*y0, 2.0f*(x0*a + y0*b), a*a + b*b, 0.0f, 0.0f);
			
			List<float> intersections = Radius.SolveRaytrace(surfaceFunction, z0, c);
			
			// The previous function returns a list of intersection distances. The value closest to 0.0f represents the
			// closest intersection point.
			float minimum = Single.PositiveInfinity;
			
			foreach (float i in intersections)
			{
				if (i > 0.0f)
				{
					minimum = Math.Sign(i)*(float)Math.Min(Math.Abs(minimum), Math.Abs(i));
				}
			}
			
			return minimum;
		}
	}
}
