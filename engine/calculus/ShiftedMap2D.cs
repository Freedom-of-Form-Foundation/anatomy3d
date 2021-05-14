using System.Collections.Generic;
using System.Numerics;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	///     <c>ShiftedMap2D<O></c> shifts the domain of a 2D map such that the values are taken at `uv + shift`.
	/// </summary>
	public class ShiftedMap2D<O> : ContinuousMap<Vector2, O>
	{
		public Vector2 Shift { get; set; }
		public ContinuousMap<Vector2, O> Function { get; set; }
		
		public ShiftedMap2D(Vector2 shift, ContinuousMap<Vector2, O> function)
		{
			this.Shift = shift;
			this.Function = function;
		}
		
		public override O GetValueAt(Vector2 t)
		{
			return Function.GetValueAt(Shift + t);
		}
	}
}
