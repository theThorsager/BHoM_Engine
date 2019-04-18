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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Physical.Properties;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Physical
{
    public static partial class Create
    {
        [Description("BH.Engine.Physical.Create.Material => Returns a Material object")]
        [Input("name", "The name of the material, default empty string")]
        [Input("density", "The density of the material, default 0.0")]
        [Input("properties", "A collection of the specific properties of the material to be created, default null")]
        [Output("A Material object")]
        public static Material Material(string name = "", double density = 0.0, List<IMaterialProperties> properties = null)
        {
            properties = properties ?? new List<IMaterialProperties>();

            return new Material
            {
                Name = name,
                Density = density,
                Properties = properties,
            };
        }
    }
}