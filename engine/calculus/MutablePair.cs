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
using System.Diagnostics;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
    /// <summary>
    /// MutablePair is a 2D point that can have its location updated. When
    /// its location is written, it calls a callback created at instantiation.
    /// </summary>
    public class MutablePair
    {
        private double _location, _value;

        public double Location
        {
            set
            {
                _location = value;
                _mutationCallback();
            }
            get => _location;
        }

        public double Value
        {
            set
            {
                _value = value;
                _mutationCallback();
            }
            get => _value;
        }
        private Action _mutationCallback;

        public MutablePair(Action mutationCallback)
        {
            Debug.Assert(!(mutationCallback is null));
            _mutationCallback = mutationCallback;
            _location = 0;
            _value = 0;
        }

        /// <summary>
        /// Set both Location and Value with only one callback.
        /// </summary>
        /// <param name="x">New Location value for the point.</param>
        /// <param name="y">New Value value for the point.</param>
        public void Set(double location, double value)
        {
            _location = location;
            _value = value;
            _mutationCallback();
        }
    }

    /// <summary>
    /// Mutable2DPointFactory generates MutablePair instances so a MutablePiecewiseInterpolatedCurve can spawn them.
    /// </summary>
    public struct Mutable2DPointFactory : IMutablePointFactory<MutablePair>
    {
        public MutablePair NewPoint(Action mutationCallback) => new MutablePair(mutationCallback);
    }
}
