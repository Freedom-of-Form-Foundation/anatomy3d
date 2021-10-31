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

using System;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	/// A ContinuousMap is an abstract class that defines some function that outputs a value of type `TOut` for each
	/// input of type `TIn`. Mathematically, this means it defines a mapping from one space to another. For example, a
	/// ContinuousMap<double, double> defines a function that takes a double value and outputs a double value, such as
	/// a polynomial function. ContinuousMap<Vector2, double> takes a Vector2 as input and gives a double as output, which
	/// could for instance be used as a height map.
	public abstract class ContinuousMap<TIn, TOut>
	{
		public abstract TOut GetValueAt(TIn t);
		
		public static implicit operator ContinuousMap<TIn, TOut>(TOut m) => new ConstantFunction<TIn, TOut>(m);

		public static implicit operator ContinuousMap<TIn, TOut>(Func<TIn, TOut> f)
		{
			return new FunctionBackedContinuousMap<TIn, TOut>(f);
		}
	}
}
