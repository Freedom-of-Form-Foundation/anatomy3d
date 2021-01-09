using System;
using System.Collections;
using System.Numerics;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public struct Vertex
	{
		public Vertex(Vector3 position)
		{
			this.Position = position;
			this.Normal = new Vector3(0.0f, 0.0f, 1.0f);
		}
		
		public Vertex(Vector3 position, Vector3 normal)
		{
			this.Position = position;
			this.Normal = normal;
		}

		public Vector3 Position {get; set;}
		public Vector3 Normal {get; set;}
		
		public override String ToString() {
			return this.Position.ToString();
		}
	}
}
