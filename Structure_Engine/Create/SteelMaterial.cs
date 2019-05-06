﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.Materials;
using BH.oM.Structure.MaterialFragments;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a steelmaterial. First constructs a Steel material fragment, then applies it to a new Material class")]
        [Output("Material", "The created material with a steel fragment")]
        public static Material SteelMaterial(string name, double E = 210000000000, double v = 0.3, double tC = 0.000012, double density = 7850, double dampingRatio = 0, double yieldStress = 0, double ultimateStress = 0)
        {
            Steel steelFragment = new Steel()
            {
                Name = name,
                YoungsModulus = E,
                PoissonsRatio = v,
                ThermalExpansionCoeff = tC,
                DampingRatio = dampingRatio,
                YieldStress = yieldStress,
                UltimateStress = ultimateStress
            };

            return  new Material()
            {
                Name = name,
                Density = density,
                Properties = new List<IMaterialProperties> { steelFragment }
            };
        }

        /***************************************************/
    }
}