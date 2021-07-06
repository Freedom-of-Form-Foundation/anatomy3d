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


/**
 * This class represents data of a single humerus. Most coordinates
 * are given in Bone Local Space, with the exception of a transformation operation
 * to orient Bone Local Space within the overall Character Space.
 */

public class HumerusDataObjectBeforeFileIO
{
	// Transformations FROM overall Character Space. These transformations would 
	// be inverted if you want to go from Bone Local Space to Character Space.
	private Numerics.Vector3 translationFromCharacterSpace = new Numerics.Vector3(0.0f, 0.0f, 0.0f);
	private Numerics.Vector3 rotationFromCharacterSpace = new Numerics.Vector3(0.0f, 0.0f, 0.0f);



	private LongitudinalStructure longitudinalStructure = new LongitudinalStructure();


	// Internal classes for now to develop the concept. These will need refactored
	// and put into appropriate places for API design.
	// I'm being reckless with class scope as well. If these are accessible
	// from outside HumerusDataObjectBeforeFileIO, PLEASE don't invoke until
	// proper refactoring is done!
	internal class ShellPrimitive
	{
		public LongitudinalStructure longitudinalStructure = new LongitudinalStructure();

		public ShellPrimitive()
		{


		}
	}

	// TODO consider splitting class into subclasses, including a subclass to represent
	// the root for LongitudinalStructure.
	internal class BoneBranchControlPoint
	{
		public Numerics.Vector3 pointPosition = new Numerics.Vector3(0f, 0f, 0f);
		public float pointRadius = 0.0f;
		public BoneBranchControlPoint otherPoint = null;

		public BoneBranchControlPoint(
			double localMedioLateralPosition,
			double localDorsoVentralPosition,
			double localAnteriorPosteriorPosition,
			float radius
		)
		{
			pointPosition = new Numerics.Vector3(
				(float)localMedioLateralPosition, //TODO - this is a cast with permanent loss of precision.
				(float)localDorsoVentralPosition,
				(float)localAnteriorPosteriorPosition
			);

			pointRadius = radius;
		}

		public Numerics.Vector3 getPointPosition()
		{
			return pointPosition;
		}

		public float getPointRadius()
		{
			return pointRadius;
		}

		// TODO. Current implementation only uses the order of BoneBranchControlPoint objects
		// within LongitudinalStructure, NOT any explicitly declared linkages between them.
		public void linkTo(BoneBranchControlPoint otherPoint)
		{

		}
	}

	internal class LongitudinalStructure
	{
		public SortedList<float, BoneBranchControlPoint> centerPoints =
					new SortedList<float, BoneBranchControlPoint>();


		// TODO this constructor runs a script with hard-set values.
		public LongitudinalStructure()
		{
			centerPoints = new SortedList<float, BoneBranchControlPoint>();

			centerPoints.Add(0.0f,
				new BoneBranchControlPoint(
					0.0d, 0.0d, 2.7d, 0.7f * 0.92f
				)
			);
			centerPoints.Add(0.25f,
				new BoneBranchControlPoint(
					-0.3f, -0.5f, 1.0f, 0.7f * 0.8f
				)
			);
			centerPoints.Add(0.5f,
				new BoneBranchControlPoint(
					0.3f, 1.0f, 0.0f, 0.7f * 0.7f
				)
			);
			centerPoints.Add(0.75f,
				new BoneBranchControlPoint(
					0.8f, 1.0f, -1.0f, 0.7f * 0.76f
				)
			);
			centerPoints.Add(1.0f,
				new BoneBranchControlPoint(
					0.6f, -0.5f, -0.9f, 0.7f * 0.8f
				)
			);
		}

		// TODO
		public void addPoint(BoneBranchControlPoint addPoint)
		{

		}


		public SpatialCubicSpline getSpline()
		{
			SortedList<float, Vector3> splineVector3Points = new SortedList<float, Vector3>();

			// TODO allow tolerance for not all BoneBranchControlPoint objects
			// to specify a position in 3D space. Some objects will only have a radius.
			foreach (float key in centerPoints.Keys())
			{
				BoneBranchControlPoint value = centerPoints.TryGetValue(key);
				splineVector3Points.Append(key, value.getPointPosition());
			}
			SpatialCubicSpline returnSpline = new SpatialCubicSpline(splineVector3Points);

			return returnSpline;
		}

		public CubicSpline1D getRadius()
		{
			SortedList<float, float> radiusPoints = new SortedList<float, float>();

			// TODO allow tolerance for not all BoneBranchControlPoint objects
			// to specify a position in 3D space. Some objects will only have a position.
			foreach (float key in centerPoints.Keys())
			{
				BoneBranchControlPoint value = centerPoints.TryGetValue(key);
				radiusPoints.Append(key, value.getPointPosition());
			}
			CubicSpline1D returnSpline = new CubicSpline1D(radiusPoints);

			return returnSpline;
		}
	}




	// TODO 
	internal class IntrinsicModifiers
	{
		// Collection<SecondaryOssificationCenter> //more complex bulging applied to the shell

	}



	// TODO
	internal class ExtrinsicModifiers
	{
		// Collection<Joint> //use InteractingJoints functionality
		// Collection<MuscleOrTendon> //influence provided by muscle-induced tension


		// Provided a linkage to a joint, along with information about
		// the type of relationship (e.g., the relationship represents
		// a negative space into which the bone should fill),
		// that should be enough information from the standpoint of THIS
		// bone.

	}



	// Temporary constructor. Once the data format is defined, this
	// constructor would be passed data rather than simply running
	// a script with hard-set values.
	public HumerusDataObjectBeforeFileIO()
	{

		longitudinalStructure = new LongitudinalStructure();
	}



	// Test method to return a current-implementation LongBone object based on the data defined within HumerusDataObjectBeforeFileIO
	// Downstream, the user should call character.bones.Add() on the returned LongBone from this method.
	public Anatomy.Bones.LongBone GET_TEST_LONGBONE()
	{
		// TODO boneAfterShellPrimitive 


		// TODO then apply intrinsic, then extrinsic modifiers before the return.
		return new Anatomy.Bones.LongBone(
			this.longitudinalStructure.getSpline,
			this.longitudinalStructure.getRadius
		);
	}
}


