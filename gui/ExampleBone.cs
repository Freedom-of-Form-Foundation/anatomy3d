using Godot;
using System;
using System.Collections.Generic;
using Numerics = System.Numerics;

using FreedomOfFormFoundation.AnatomyEngine;
using Anatomy = FreedomOfFormFoundation.AnatomyEngine.Anatomy;
using FreedomOfFormFoundation.AnatomyEngine.Anatomy.Bones;
using FreedomOfFormFoundation.AnatomyEngine.Geometry;
using FreedomOfFormFoundation.AnatomyEngine.Calculus;
using FreedomOfFormFoundation.AnatomyEngine.Renderable;

namespace FreedomOfFormFoundation.AnatomyRenderer
{
	public class ExampleBone : MeshInstance
	{
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			// Example method that creates a character and adds a single joint and bone:
			Character character = new Character();
			
			// Generate a simple cubic spline that will act as the radius of a long bone:
			SortedList<float, float> splinePoints = new SortedList<float, float>();
			splinePoints.Add(0.0f, 0.7f*0.96f);
			splinePoints.Add(0.02f, 0.7f*0.96f);
			splinePoints.Add(0.15f, 0.7f*0.8f);
			splinePoints.Add(0.5f, 0.7f*0.8f);
			splinePoints.Add(0.8f, 0.7f*0.9f);
			splinePoints.Add(0.98f, 0.7f*1.2f);
			splinePoints.Add(1.0f, 0.7f*1.2f);
			
			CubicSpline1D spline = new CubicSpline1D(splinePoints);

			// Define the center curve of the long bone:
			Line centerLine = new Line(new Numerics.Vector3(0.5f, -0.2f, 1.7f),
									   new Numerics.Vector3(0f, 0.2f, -1.7f));
			
			// Add a long bone to the character:
			character.bones.Add(new Anatomy.Bones.LongBone(centerLine, spline));
			
			// Generate the geometry vertices of the first bone with resolution U=32 and resolution V=32:
			UVMesh mesh = character.bones[0].GetGeometry().GenerateMesh(64, 64);
			
			// Finally upload the mesh to Godot:
			this.Mesh = new GodotMeshConverter(mesh);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(float delta)
		{
			// Visually spin the bone around, just for show:
			RotateY(0.5f*delta);
		}
	}
}
