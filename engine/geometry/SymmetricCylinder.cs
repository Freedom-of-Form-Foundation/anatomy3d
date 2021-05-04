using System.Collections.Generic;
using System.Numerics;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public class SymmetricCylinder : Cylinder
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
	}
}
