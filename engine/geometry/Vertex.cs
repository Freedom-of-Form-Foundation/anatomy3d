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
        }

        public Vector3 Position {get; set;}
        
        public override String ToString() {
            return this.Position.ToString();
        }
    }
}
