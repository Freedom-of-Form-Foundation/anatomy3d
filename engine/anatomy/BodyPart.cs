using System.Collections;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Anatomy
{
	public abstract class BodyPart
	{
		/// <summary>
		///     Returns the surface geometry used by this IBodyPart.
		/// </summary>
		public abstract Surface GetGeometry();
	}
}
