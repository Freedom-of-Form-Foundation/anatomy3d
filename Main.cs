using System;
using System.Collections.Generic;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Anatomy;
using FreedomOfFormFoundation.AnatomyEngine.Anatomy.Bones;
using FreedomOfFormFoundation.AnatomyEngine.Anatomy.Joints;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;

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
            
            Console.WriteLine("VBO:");
            
            // Generate the geometry vertices of the first bone with resolution U=32 and resolution V=32:
            List<Vertex> VBO = character.bones[0].GetGeometry().GenerateVertexList(32, 32);
            
            // Print the vertex list to see if the contents make sense:
            foreach (Vertex i in VBO)
            {
                Console.WriteLine(i);
            }
            
            Console.WriteLine("IBO: ");
            
            // Generate the geometry vertices of the first bone with resolution U=32 and resolution V=32:
            List<int> IBO = character.bones[0].GetGeometry().GenerateIndexList(32, 32);
            
            // Print the vertex list to see if the contents make sense:
            foreach (int i in IBO)
            {
                Console.Write(i);
                Console.Write(" ");
            }
            
            Console.WriteLine("\nFinished example!");
        }
    }
}
