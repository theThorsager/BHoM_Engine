﻿/*
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Fragments;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.oM.Geometry.SettingOut;

using BH.Engine.Base;

using BH.oM.Physical.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of Environment Panels that sit entirely on a given levels elevation")]
        [Input("panels", "A collection of Environment Panels to filter")]
        [Input("searchLevel", "The Setting Out Level to search by")]
        [Output("panels", "A collection of Environment Panels which match the given level")]
        public static List<Panel> PanelsByLevel(this List<Panel> panels, Level searchLevel)
        {
            return panels.PanelsByLevel(searchLevel.Elevation);
        }

        [Description("Returns a collection of Environment Panels that sit entirely on a given levels elevation")]
        [Input("panels", "A collection of Environment Panels to filter")]
        [Input("searchLevel", "The level to search by")]
        [Output("panels", "A collection of Environment Panels which match the given level")]
        public static List<Panel> PanelsByLevel(this List<Panel> panels, double searchLevel)
        {
            return panels.Where(x => x.MinimumLevel() == searchLevel && x.MaximumLevel() == searchLevel).ToList();
        }

    }
}
