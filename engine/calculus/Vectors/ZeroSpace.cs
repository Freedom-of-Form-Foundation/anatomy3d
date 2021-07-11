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
    /// ZeroSpace is the zero-dimensional vector space.
    /// </summary>
    public struct ZeroSpace : IVectorSpace
    {
        public bool Equals(IVectorSpace other)
        {
            return other is ZeroSpace;
        }

        public int Dimension() => 0;
    }
}
