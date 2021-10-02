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
using System.Numerics;
using System.Linq;
using System;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// <summary>
	/// A <c>SortedPointsList</c> stores points in <c>List</c>s so that BinarySearch can be used. The points in this
	/// list are guaranteed to be sorted by Location-value and to have no duplicate entries for the Location-coordinate. This is a
	/// bit roundabout, and should probably be replaced once a BinarySearch is added for IList or SortedList.
	/// </summary>
	public class SortedPointsList<T>
	{
		public List<T> Key { get; }
		public List<T> Value { get; }
		public Int32 Count { get; }
		
		public SortedPointsList(SortedList<T, T> points)
		{
			Key = points.Keys.ToList();
			Value = points.Values.ToList();
			Count = points.Count;
		}
	}
}
