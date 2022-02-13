using System.Collections.Generic;
using System.Numerics;
using FreedomOfFormFoundation.AnatomyEngine.Anatomy;

namespace FreedomOfFormFoundation.AnatomyEngine
{
	public class Species
	{
		public List<Anatomy.Joint> joints = new List<Joint>();
		public List<Anatomy.Bone> bones = new List<Bone>();
	}
}
