using System.Collections.Generic;
using System.Numerics;
using System;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	public abstract class ContinuousMap<I, O>
	{
		public abstract O GetValueAt(I t);
		
		public static implicit operator ContinuousMap<I, O>(O m) => new ConstantFunction<I, O>(m);
		
		/// <summary>
		///     Solves the equation \f$(q(x))^2 = b_0 + b_1 x + b_2 x^2 + b_3 x^3 + b_4 x^4\f$, returning all values of
		///		\f$x\f$ for which the equation is true. \f$q(x)\f$ is the continuous map. The parameters z0 and c
		///		can be used to substitute x, such that \f$x = z0 + c t\f$. This is useful for raytracing.
		/// </summary>
		public virtual List<float> SolveRayTrace(float b0, float b1, float b2, float b3, float b4, float z0 = 0.0f, float c = 1.0f)
		{
			throw new NotImplementedException();
		}
	}
}
