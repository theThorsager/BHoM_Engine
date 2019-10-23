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

using BH.oM.Base;
using BH.oM.Data.Collections;
using BH.oM.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.Engine.Serialiser;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Diffing
{
    public static partial class Create
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/

        [Description("Defines a set of configurations of the diffing.")]
        [Input("enablePropertyDiffing", "Enables or disables the property-by-property diffing. If false, only collection-level diffing will be performed.")]
        public static DiffConfig DiffConfig(bool enablePropertyDiffing = true)
        {
            return new DiffConfig() { EnablePropertyDiffing = enablePropertyDiffing };
        }

        [Description("Defines a set of configurations of the diffing.")]
        [Input("propertiesToIgnore", "List of strings specifying the names of the properties that should be ignored in the diffing.\nBy default: 'BHoM_Guid', 'CustomData', 'Fragments`")]
        [Input("enablePropertyDiffing", "If false, only collection-level diffing will be performed, otherwise property-by-property. Defaults to true.")]
        public static DiffConfig DiffConfig(List<string> propertiesToIgnore, bool enablePropertyDiffing = true)
        {
            return new DiffConfig() { PropertiesToIgnore = propertiesToIgnore, EnablePropertyDiffing = enablePropertyDiffing };
        }


    }
}