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

namespace FreedomOfFormFoundation.AnatomyEngine.Geometry
{
    /// <summary>
    /// Mutable2DPoint is a 2D point that can have its location updated. When
    /// its location is written, it calls a callback created at instantiation.
    /// </summary>
    public class Mutable2DPoint
    {
        private Real _x, _y;

        public Real X
        {
            set
            {
                _x = value;
                _mutationCallback();
            }
            get => _x;
        }

        public Real Y
        {
            set
            {
                _y = value;
                _mutationCallback();
            }
            get => _y;
        }
        private Action _mutationCallback;

        public Mutable2DPoint(Action mutationCallback)
        {
            Debug.Assert(!(mutationCallback is null));
            _mutationCallback = mutationCallback;
            _x = 0;
            _y = 0;
        }

        /// <summary>
        /// Set both X and Y with only one callback.
        /// </summary>
        /// <param name="x">New X value for the point.</param>
        /// <param name="y">New Y value for the point.</param>
        public void Set(Real x, Real y)
        {
            _x = x;
            _y = y;
            _mutationCallback();
        }
    }

    public class Mutable2DPointFactory : IMutablePointFactory<Mutable2DPoint>
    {
        public Mutable2DPoint NewPoint(Action mutationCallback) => new Mutable2DPoint(mutationCallback);
    }
}
