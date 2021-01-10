using System.Collections.Generic;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Anatomy;

namespace FreedomOfFormFoundation.AnatomyEngine
{
	public class Character
	{
		public List<Anatomy.Joint> joints;
		public List<Anatomy.Bone> bones;
		
		public Character()
		{
			joints = new List<Anatomy.Joint>();
			bones = new List<Anatomy.Bone>();
		}
	}
}
