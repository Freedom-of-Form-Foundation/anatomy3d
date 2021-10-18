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
using GlmSharp;

using FreedomOfFormFoundation.AnatomyEngine.Renderable;

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
	public abstract class Surface
	{
		/// <summary>
		///     Returns the surface normal at the uv coordinate given.
		/// </summary>
		public abstract dvec3 GetNormalAt(dvec2 uv);
		
		/// <summary>
		///     Returns the surface position at the uv coordinate given.
		/// </summary>
		public abstract dvec3 GetPositionAt(dvec2 uv);
		
		/// <summary>
		///     Generates the vertex positions of this uv-parametrized surface. The returned list will contain
		///     approximately <paramref name="resolutionU"/> times <paramref name="resolutionV"/> vertices in total. The
		///     meaning of the coordinates u and v depends on the specific type of surface.
		/// </summary>
		public abstract List<Vertex> GenerateVertexList(int resolutionU, int resolutionV);
		
		/// <summary>
		///     Generates the indices pointing to vertices of this uv-parametrized surface. The returned list can
		///     contain any number of indices, since there will be a variable number of triangles depending on the
		///     surface type.
		/// </summary>
		public abstract List<int> GenerateIndexList(int resolutionU, int resolutionV, int indexOffset = 0);
		
		/// <summary>
		///     Generates the vertex positions and indices of this uv-parametrized surface. The returned list will
		///     contain approximately <paramref name="resolutionU"/> times <paramref name="resolutionV"/> vertices in
		///     total. The meaning of the coordinates u and v depends on the specific type of surface.
		/// </summary>
		public virtual UVMesh GenerateMesh(int resolutionU, int resolutionV, int indexOffset = 0)
		{
			return new UVMesh(this, resolutionU, resolutionV, indexOffset);
		}
	}
}
