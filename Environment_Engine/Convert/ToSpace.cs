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

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Convert
    {
        [Description("Convert a BHoM Boundary Representation (Brep) into a closed volume of Environmnts Panels which form a space. Useful for converting geometry from volumes to Panels for model building")]
        [Input("brep", "A BHoM Boundary Representation to convert into a collection of Environment Panels")]
        [Input("connectedSpaceName", "A name for the space which these panels are connected to. If no name is provided, a randomised default will be generated")]
        [Input("angleTolerance", "The angle tolerance for collapsing to polylines used when generating the external edges of the surfaces")]
        [Output("panelsAsSpace", "A collection of Environment Panels representing a closed space generated from the provided Brep geometry")]
        public static List<Panel> ToPanels(this BoundaryRepresentation brep, string connectedSpaceName = null, double angleTolerance = BH.oM.Geometry.Tolerance.Angle)
        {
            return brep.Surfaces.ToList().ToPanels(connectedSpaceName, angleTolerance);
        }

        [Description("Convert a collection of BHoM surfaces into a collection of Environmnts Panels")]
        [Input("surfaces", "A collection of BHoM surfaces to convert into a Environment Panels. The surfaces should be grouped as a single space as all panels generated from the surfaces will have the same connectedSpaceName")]
        [Input("connectedSpaceName", "A name for the space which these panels are connected to. If no name is provided, a randomised default will be generated")]
        [Input("angleTolerance", "The angle tolerance for collapsing to polylines used when generating the external edges of the surfaces")]
        [Output("panel", "An Environment Panels representing a closed space generated from the provided surfaces")]
        public static List<Panel> ToPanels(this List<ISurface> surfaces, string connectedSpaceName = null, double angleTolerance = BH.oM.Geometry.Tolerance.Angle)
        {
            if (connectedSpaceName == null)
                connectedSpaceName = Guid.NewGuid().ToString();

            List<Panel> panels = surfaces.Select(x => x.ToPanel(connectedSpaceName, angleTolerance)).ToList();

            panels = panels.SetRoofPanels();
            panels = panels.SetFloorPanels();
            panels = panels.SetWallPanels();

            return panels;
        }

        [Description("Convert a BHoM surface into an Environmnts Panel")]
        [Input("surface", "A BHoM surface to convert into an Environment Panel")]
        [Input("connectedSpaceName", "A name for the space which this panel is connected to. If no name is provided, a randomised default will be generated")]
        [Input("angleTolerance", "The angle tolerance for collapsing to polylines used when generating the external edges of the surfaces")]
        [Output("panel", "An Environment Panels representing a closed space generated from the provided Brep geometry")]
        public static Panel ToPanel(this ISurface surface, string connectedSpaceName = null, double angleTolerance = BH.oM.Geometry.Tolerance.Angle)
        {
            if (connectedSpaceName == null)
                connectedSpaceName = Guid.NewGuid().ToString();

            return new Panel
            {
                ExternalEdges = surface.IExternalEdges().Select(x => x.ICollapseToPolyline(angleTolerance)).ToList().Join().ToEdges(),
                ConnectedSpaces = new List<string> { connectedSpaceName },
            };
        }
    }
}
