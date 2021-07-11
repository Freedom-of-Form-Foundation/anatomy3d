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

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus.Vectors
{
    /// <summary>
    /// WorldSpace is a 3-dimensional Cartesian coordinate space representing X, Y, and Z, serving as the absolute
    /// spatial reference for the world as a whole.
    /// </summary>
    public struct WorldSpace : IVectorSpace
    {
        public bool Equals(IVectorSpace other) => other is WorldSpace;

        public int Dimension() => 3;

        /// <summary>
        /// Ordinal of the X axis.
        /// </summary>
        public const int X = 0;

        /// <summary>
        /// Ordinal of the Y axis.
        /// </summary>
        public const int Y = 1;

        /// <summary>
        /// Ordinal of the Z axis.
        /// </summary>
        public const int Z = 2;
    }
}
