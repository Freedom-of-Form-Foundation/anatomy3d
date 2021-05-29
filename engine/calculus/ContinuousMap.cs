using System.Collections.Generic;
using System.Numerics;
using System;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	public abstract class ContinuousMap<I, O>
	{
		public abstract O GetValueAt(I t);
		
		public static implicit operator ContinuousMap<I, O>(O m) => new ConstantFunction<I, O>(m);

		public static implicit operator ContinuousMap<I, O>(Func<I, O> f)
		{
			return new FunctionBackedContinuousMap<I, O>(f);
		}
	}
}
