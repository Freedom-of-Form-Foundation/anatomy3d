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

using System.Diagnostics;

namespace FreedomOfFormFoundation.AnatomyEngine
{
    /// <summary>
    /// DebugUtil contains assorted <c>void</c> methods that are not compiled
    /// into release builds. They contain various assertions and checks for
    /// potential bugs that should be detected as close as possible to where
    /// they arise, but the performance impact of these checks would be
    /// inappropriate in a release build.
    /// </summary>
    public static class DebugUtil
    {
        /// <summary>
        /// AssertFinite crashes the program (in debug mode) if <c>x</c> is infinite or NaN.
        /// It does not object to negative zero.
        /// </summary>
        /// <param name="x">Value to range check.</param>
        /// <param name="varName">Name of variable being checked (use nameof); included in error message. </param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void AssertFinite(double x, string varName)
        {
            Debug.Assert(
                !double.IsInfinity(x) && !double.IsNaN(x),
                "Transfinite double error",
                "Variable {0}, expected to be finite, had illegal value {1}.",
                varName, x);
        }
    }
}
