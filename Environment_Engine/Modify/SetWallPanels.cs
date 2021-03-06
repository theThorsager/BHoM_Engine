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

using System.Linq;
using System.Collections.Generic;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Environment.Fragments;
using System;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the wall panels of a space represented by Environment Panels and fixes PanelType")]
        [Input("panelsAsSpace", "A collection of Environment Panels that represent a closed space")]
        [Output("wallPanels", "BHoM Environment panel representing the wall of the space")]
        public static List<Panel> SetWallPanels(this List<Panel> panelsAsSpace)
        {
            List<Panel> clones = new List<Panel>(panelsAsSpace.Select(x => x.DeepClone<Panel>()).ToList());

            //Find the panel(s) that are horizontal
            double minZ = 1e10;
            foreach (Panel panel in clones)
            {
                if (panel.MinimumLevel() == panel.MaximumLevel())
                    minZ = Math.Min(minZ, panel.MinimumLevel());
            }

            List<Panel> wallPanels = clones.Where(x => x.Tilt() < 92 && x.Tilt() > 88).ToList();

            if (wallPanels.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Could not find wall panel");
                return null;
            }

            foreach (Panel panel in wallPanels)
            {
                if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 1)
                    panel.Type = PanelType.WallExternal;
                else if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 2)
                    panel.Type = PanelType.WallInternal;
            }

            return wallPanels;
        }
    }
}

