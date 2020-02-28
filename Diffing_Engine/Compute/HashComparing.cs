/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.Engine;
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
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Reflection;

namespace BH.Engine.Diffing
{
    public static partial class Compute
    {
        [Description("Dispatch objects in two sets into the ones exclusive to one set, the other, or both.")]
        [Input("set1", "A previous version of a list of IBHoMObjects")]
        [Input("set2", "A new version of a list of IBHoMObjects")]
        [Input("diffConfig", "Sets configs such as properties to be ignored in the comparison.")]
        [Output("VennDiagram", "Venn diagram containing: objects existing exclusively in set1/set2 or their intersection.")]
        public static VennDiagram<T> HashComparing<T>(IEnumerable<T> set1, IEnumerable<T> set2, DiffConfig diffConfig = null) where T : class, IBHoMObject
        {
            IEnumerable<T> set1Cloned = Modify.PrepareForDiffing(set1);
            IEnumerable<T> set2Cloned = Modify.PrepareForDiffing(set2);

            return Engine.Data.Create.VennDiagram(set1Cloned, set2Cloned, new HashFragmComparer<T>());
        }
    }
}

