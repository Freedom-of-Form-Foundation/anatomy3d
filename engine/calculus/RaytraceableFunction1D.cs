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

using System.Collections.Generic;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
	public abstract class RaytraceableFunction1D : ContinuousMap<double, double>
	{
		/// <summary>
		/// Solves the equation \f$(q(x))^2 = b_0 + b_1 x + b_2 x^2 + b_3 x^3 + b_4 x^4\f$, returning all values of
		///	\f$x\f$ for which the equation is true. \f$q(x)\f$ is the continuous map. The parameters z0 and c
		///	can be used to substitute x, such that \f$x = z0 + c t\f$. This is useful for raytracing.
		/// </summary>
		public abstract IEnumerable<double> SolveRaytrace(QuarticFunction surfaceFunction, double z0 = 0.0, double c = 1.0);
	}
}
