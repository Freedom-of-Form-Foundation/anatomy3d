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
    public class FunctionBackedContinuousMap<TIn, TOut> : ContinuousMap<TIn, TOut>
    {
        /// <summary>
        /// Function used to implement GetValueAt.
        /// </summary>
        public Func<TIn, TOut> F { get; }

        public FunctionBackedContinuousMap(Func<TIn, TOut> f)
        {
            F = f;
        }

        /// <summary>
        /// Evaluates this object's wrapped function at the provided value.
        /// </summary>
        /// <param name="t">Value to call function with.</param>
        /// <returns>Return value of wrapped function.</returns>
        public override TOut GetValueAt(TIn t)
        {
            return F(t);
        }

        public static implicit operator FunctionBackedContinuousMap<TIn, TOut>(Func<TIn, TOut> f)
        {
            return new FunctionBackedContinuousMap<TIn, TOut>(f);
        }
    }
}
