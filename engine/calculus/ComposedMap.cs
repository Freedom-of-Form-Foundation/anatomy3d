﻿/*
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
	// ComposedMap represents one mapping function composed with another, such that the left map "feeds into" the
	// right map. This map behaves like right(left(x)). It's intended to be constructed by the convenience
	// extension method "Then" - see ContinuousMapExtensions.cs for details - where automatic type deduction in
	// method calls will get rid of the type boilerplate.
	public sealed class ComposedMap<TIn, TIntermediate, TOut> : ContinuousMap<TIn, TOut>
	{
		/// <summary>
		/// First map to use, calculating a final value.
		/// </summary>
		private readonly ContinuousMap<TIn, TIntermediate> _left;

		/// <summary>
		/// Second map to use, calculating a final value from the intermediate value.
		/// </summary>
		private readonly ContinuousMap<TIntermediate, TOut> _right;

		/// <summary>
		/// Constructs a ComposedMap feeding the left map into the right map to produce a map from left.I to right.O.
		/// Due to the general ugliness of ComposedMap's full type name, you probably don't want to use this directly.
		/// Use left.Then(right) instead.
		/// </summary>
		/// <param name="left">Map from input value to an intermediate value. Nonnull.</param>
		/// <param name="right">Map from intermediate value to the final value. Nonnull.</param>
		/// <exception cref="ArgumentException">If either argument is null.</exception>
		public ComposedMap(ContinuousMap<TIn, TIntermediate> left,
			ContinuousMap<TIntermediate, TOut> right)
		{
			if (left == null)
			{
				throw new ArgumentException("ComposedMap constructor requires nonnull arguments", "left");
			}

			if (right == null)
			{
				throw new ArgumentException("ComposedMap constructor requires nonnull arguments", "right");
			}

			_left = left;
			_right = right;
		}

		/// <summary>
		/// Gets a value from the second continuous map this object was constructed with by giving it the output of
		/// the first map, which is given the input to this function. Composes functions.
		/// </summary>
		/// <param name="t">Value to give to first map.</param>
		/// <returns>Value of second map at the first map's value for t.</returns>
		public override TOut GetValueAt(TIn t) => _right.GetValueAt(_left.GetValueAt(t));
	}
}
