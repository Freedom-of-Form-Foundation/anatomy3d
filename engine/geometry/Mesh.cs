/*
 * Copyright (C) 2021 Freedom of Form Foundation, Inc.
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License, version 2 (GPLv2) as published by the Free Software Foundation.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License, version 2 (GPLv2) for more details.
 * 
 * You should have received a copy of the GNU General Public License, version 2 (GPLv2)
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System.Collections.Generic;

using FreedomOfFormFoundation.AnatomyEngine.Geometry;

namespace FreedomOfFormFoundation.AnatomyEngine.Renderable
{
	public class UVMesh
	{
		public UVMesh(Surface surface, int resolutionU, int resolutionV, int indexOffset = 0)
		{
			VertexList = surface.GenerateVertexList(resolutionU, resolutionV);
			IndexList = surface.GenerateIndexList(resolutionU, resolutionV, indexOffset);
			ResolutionU = resolutionU;
			ResolutionV = resolutionV;
		}
		
		public int ResolutionU { get; }
		public int ResolutionV { get; }
		
		/// <summary>
		///     The list containing 3D positions and auxiliary data of each vertex in the mesh. Relates to VBO.
		/// </summary>
		public List<Vertex> VertexList { get; set; }
		
		/// <summary>
		///     The list containing the indices of vertices, where each group of three indices corresponds to a single
		///     triangle. Relates to IBO.
		/// </summary>
		public List<int> IndexList { get; set; }
	}
}
