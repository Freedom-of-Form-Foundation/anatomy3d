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
			float x0 = rayStart.X;
			float y0 = rayStart.Y;
			float z0 = rayStart.Z;
			
			float a = rayDirection.X;
			float b = rayDirection.Y;
			float c = rayDirection.Z;
			
			// Raytrace using a cylindrical surface equation x^2 + y^2. The parameters in the following line
			// represent the coefficients of the expanded cylindrical surface equation, after the substitution
			// x = x_0 + a t and y = y_0 + b t:
			List<float> intersections = Radius.SolveRayTrace(x0*x0 + y0*y0, 2.0f*(x0*a + y0*b), a*a + b*b, 0.0f, 0.0f, z0, c);
			
			// The previous function returns a list of intersection distances. The value closest to 0.0f represents the
			// closest intersection point.
			float minimum = Single.PositiveInfinity;
			
			Console.WriteLine("intersections: " + intersections.Count);
			
			foreach (float i in intersections)
			{
				if (i > 0.0f)
				{
					minimum = Math.Sign(i)*(float)Math.Min(Math.Abs(minimum), Math.Abs(i));
					Console.WriteLine(i);
				}
			}
			
			return Single.IsInfinity(minimum) ? 0.0f : minimum;
		}
	}
}