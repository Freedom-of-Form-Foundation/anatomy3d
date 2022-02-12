using Godot;
using System;
using System.Collections.Generic;
using GlmSharp;

using Anatomy = FreedomOfFormFoundation.AnatomyEngine.Anatomy;
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
			Anatomy.Skeleton skeleton = new Anatomy.Skeleton();
			
			CreateExampleJoint(skeleton);
			CreateExampleBones(skeleton);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(float delta)
		{
			
		}
		
		public void CreateExampleBones(Anatomy.Skeleton skeleton)
		{
			// Generate a simple cubic spline that will act as the radius of a long bone:
			SortedList<double, double> radiusPoints = new SortedList<double, double>();
			radiusPoints.Add(-3.5f, 0.7f*1.2f);
			radiusPoints.Add(-1.0f, 0.7f*1.2f);
			radiusPoints.Add(0.02f, 0.7f*1.2f);
			radiusPoints.Add(0.15f, 0.7f*1.0f);
			radiusPoints.Add(0.5f, 0.7f*0.7f);
			radiusPoints.Add(0.8f, 0.7f*0.76f);
			radiusPoints.Add(0.98f, 0.7f*0.8f);
			radiusPoints.Add(4.5f, 0.7f*0.8f);
			
			LinearSpline1D humerusRadius = new LinearSpline1D(radiusPoints);
			
			// Generate a simple cubic spline that will act as the radius of a long bone:
			SortedList<double, double> radiusPoints2 = new SortedList<double, double>();
			radiusPoints2.Add(-3.5f, 0.7f*0.3f);
			radiusPoints2.Add(-1.0f, 0.7f*0.3f);
			radiusPoints2.Add(0.02f, 0.7f*0.3f);
			radiusPoints2.Add(0.15f, 0.7f*1.0f);
			radiusPoints2.Add(0.5f, 0.7f*0.7f);
			radiusPoints2.Add(0.8f, 0.7f*0.76f);
			radiusPoints2.Add(0.98f, 0.7f*0.8f);
			radiusPoints2.Add(4.5f, 0.7f*0.8f);
			
			LinearSpline1D ulnaRadius = new LinearSpline1D(radiusPoints2);

			// Define the center curve of the long bone:
			SortedList<double, dvec3> centerPoints = new SortedList<double, dvec3>();
			centerPoints.Add(0.0f, new dvec3(0.0f, 0.0f, 2.7f));
			centerPoints.Add(0.25f, new dvec3(-0.3f, -0.5f, 1.0f));
			centerPoints.Add(0.5f, new dvec3(0.3f, 1.0f, 0.0f));
			centerPoints.Add(0.75f, new dvec3(0.8f, 1.0f, -1.0f));
			centerPoints.Add(1.0f, new dvec3(0.6f, -0.5f, -0.9f));
			
			SpatialCubicSpline boneCenter = new SpatialCubicSpline(centerPoints);
			
			// Add first bone:
			LineSegment centerLine = new LineSegment(new dvec3(0.0f, 0.0f, 0.5f),
									   new dvec3(0.001f, 10.0f, 0.51f));
			
			var bone1 = new Anatomy.Bones.LongBone(centerLine, humerusRadius);
			
			var jointInteraction = new Anatomy.Bone.JointDeformation(skeleton.joints[0], RayCastDirection.Outwards, 3.0f, 0.0f);
			bone1.InteractingJoints.Add(jointInteraction);
			skeleton.bones.Add(bone1);
			
			// Add second bone:
			LineSegment centerLine2 = new LineSegment(new dvec3(-0.5f, -1.4f, 0.5f),
									   new dvec3(10.0f, -1.4f, 0.5f));
			
			var bone2 = new Anatomy.Bones.LongBone(centerLine2, ulnaRadius);
			var jointInteraction2 = new Anatomy.Bone.JointDeformation(skeleton.joints[0], RayCastDirection.Outwards, 3.0f, 5.0f);
			bone2.InteractingJoints.Add(jointInteraction2);
			skeleton.bones.Add(bone2);
			
			// Generate the geometry vertices of the first bone with resolution U=128 and resolution V=128:
			foreach ( var bone in skeleton.bones )
			{
#if GODOT_HTML5
				UVMesh mesh = bone.GetGeometry().GenerateMesh(32, 32);
#else
				UVMesh mesh = bone.GetGeometry().GenerateMesh(256, 256);
#endif
				
				// Finally upload the mesh to Godot:
				MeshInstance newMesh = new MeshInstance();
				newMesh.Mesh = new GodotMeshConverter(mesh);
				
				// Give each mesh a random color:
				var boneMaterial = (SpatialMaterial)GD.Load("res://BoneMaterial.tres").Duplicate();
				boneMaterial.AlbedoColor = new Color(GD.Randf(), GD.Randf(), GD.Randf(), GD.Randf());
				newMesh.SetSurfaceMaterial(0, boneMaterial);
				
				AddChild(newMesh);
			}
		}
		
		public void CreateExampleJoint(Anatomy.Skeleton skeleton)
		{
			// Generate a simple cubic spline that will act as the radius of a long bone:
			SortedList<double, double> splinePoints = new SortedList<double, double>();
			double radiusModifier = 0.6f;
			splinePoints.Add(-0.1f, radiusModifier*1.1f);
			splinePoints.Add(0.0f, radiusModifier*1.1f);
			splinePoints.Add(0.15f, radiusModifier*0.95f);
			splinePoints.Add(0.3f, radiusModifier*0.9f);
			splinePoints.Add(0.5f, radiusModifier*1.15f);
			splinePoints.Add(0.7f, radiusModifier*0.95f);
			splinePoints.Add(0.8f, radiusModifier*0.95f);
			splinePoints.Add(1.0f, radiusModifier*1.1f);
			
			QuadraticSpline1D jointSpline = new QuadraticSpline1D(splinePoints);

			// Define the center curve of the long bone:
			//LineSegment centerLine = new LineSegment(new dvec3(0.0f, -0.5f, 0.0f),
			//						new dvec3(0.0f, 1.5f, 0.5f));
									
			LineSegment centerLine = new LineSegment(new dvec3(0.0f, 0.0f, -0.5f),
									new dvec3(0.0f, 0.0f, 1.5f));
			
			// Add a long bone to the character:
			skeleton.joints.Add(new Anatomy.Joints.HingeJoint(centerLine, jointSpline, 0.0f, 1.0f*(double)Math.PI));

			// Generate the geometry vertices of the first bone with resolution U=32 and resolution V=32:
#if GODOT_HTML5
			UVMesh mesh = skeleton.joints[0].GetArticularSurface().GenerateMesh(8, 8);
#else
			UVMesh mesh = skeleton.joints[0].GetArticularSurface().GenerateMesh(64, 64);
#endif

			// Finally upload the mesh to Godot:
			MeshInstance newMesh = new MeshInstance();
			newMesh.Mesh = new GodotMeshConverter(mesh);
			newMesh.SetSurfaceMaterial(0, (Material)GD.Load("res://JointMaterial.tres"));
			
			AddChild(newMesh);
		}
	}
}
