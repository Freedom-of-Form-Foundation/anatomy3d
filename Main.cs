using System;
using System.Collections.Generic;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Anatomy;
using FreedomOfFormFoundation.AnatomyEngine.Anatomy.Bones;
using FreedomOfFormFoundation.AnatomyEngine.Anatomy.Joints;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;
using FreedomOfFormFoundation.AnatomyEngine.Renderable;

namespace FreedomOfFormFoundation.AnatomyEngine
{
    class Example {         
        static void Main()
        {
            Console.WriteLine("Starting example!");
            
            // Example method that creates a character and adds a single joint and bone:
            Character character = new Character();
            
            // Add a hinge joint with the hinging axis defined by two end points, and radius 1.0f:
            Vector3 start = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 end = new Vector3(0.0f, 1.0f, 0.0f);
            float radius = 1.0f;
            
            character.joints.Add(new Anatomy.Joints.HingeJoint(new Line(start, end), radius));
            
            // Add a long bone to the character:
            character.bones.Add(new Anatomy.Bones.LongBone());
            
            // Generate the geometry vertices of the first bone with resolution U=32 and resolution V=32:
            UVMesh mesh = character.bones[0].GetGeometry().GenerateMesh(32, 32);
            
            // Print the vertex list to see if the contents make sense:
            Console.WriteLine("Vertices:");
            foreach (Vertex i in mesh.VertexList)
            {
                Console.WriteLine(i);
            }
            
            // Print the index list to see if the contents make sense:
            Console.WriteLine("Indices:");
            foreach (int i in mesh.IndexList)
            {
                Console.Write(i);
                Console.Write(" ");
            }
            
            Console.WriteLine("\nFinished example!");
        }
    }
}
