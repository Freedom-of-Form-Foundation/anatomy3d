using System.Collections.Generic;
using System.Numerics;
using System;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	///     <c>DomainToVector2<O></c> allows a function on a lower-dimensional domain to be used in places that
	///		expect a higher-dimensional domain. For example, one might want to use a 1D function to describe the
	///		height of a 2D plane along just a single axis (such as the shape of a corrugated sheet). Since the 2D
	///		plane expects a 2D function to describe its height, one cannot use a 1D function to describe its height
	///		properly, since it is unknown which axis the function should use. <c>DomainToVector2<O></c> solves this
	///		issue by stretching a 1D function onto a 2D domain.
	/// </summary>
	/// <param name="direction">
	/// 	The direction in which the 1D function is placed onto the 2D domain.
	/// </param>
	public class DomainToVector2<O> : ContinuousMap<Vector2, O>
	{
		public Vector2 ParameterDirection { get; set; }
		public ContinuousMap<float, O> Function { get; set; }
		
		/// <summary>
		///		Constructs a ContinuousMap taking 2D input values, based on a ContinuousMap that takes 1D input values.
		/// 	<example>For example:
		/// 	<code>
		///			// Define an arbitrary 1D function:
		/// 		ContinuousMap<float, float> lineFunction = new QuadraticFunction(1.0f, 1.0f, 1.0f);
		///			
		///			Vector2 direction = new Vector2(0.0f, 1.0f); // Let the 2D function vary along the y-axis.
		/// 		ContinuousMap<Vector2, float> planeFunction = new DomainToVector2<float>(direction, function);
		/// 	</code>
		/// 	creates a 2D function described by a 1D quadratic function along the y-axis. The x-axis of the resulting
		///		function is therefore constant, while the output value varies along the y-axis, as specified by
		///		<c>direction</c>.
		/// 	</example>
		/// </summary>
		/// <param name="direction">
		/// 	The direction in which the 1D function is placed onto the 2D domain.
		/// </param>
		public DomainToVector2(Vector2 direction, ContinuousMap<float, O> function)
		{
			this.ParameterDirection = direction;
			this.Function = function;
		}
		
		public override O GetValueAt(Vector2 t)
		{
			return Function.GetValueAt(Vector2.Dot(ParameterDirection, t));
		}
		
		public override List<float> SolveRayTrace(float b0, float b1, float b2, float b3, float b4, float z0 = 0.0f, float c = 1.0f)
		{
			return Function.SolveRayTrace(b0, b1, b2, b3, b4, z0, c);
		}
	}
}
