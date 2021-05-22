using System.Collections.Generic;
using System.Numerics;
using System;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	public class ConstantFunction<I, O> : ContinuousMap<I, O>
	{
		O constant;
		
		public ConstantFunction(O constant)
		{
			this.constant = constant;
		}
		
		public override O GetValueAt(I t)
		{
			return constant;
		}
	}
}
