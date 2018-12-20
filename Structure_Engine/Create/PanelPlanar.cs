/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties.Surface;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PanelPlanar PanelPlanar(ICurve outline, List<Opening> openings = null, ISurfaceProperty property = null, string name = "")
        {
            if (!outline.IIsClosed()) return null;
            List<Edge> externalEdges = outline.ISubParts().Select(x => new Edge { Curve = x }).ToList();

            return PanelPlanar(externalEdges, openings, property, name);
        }

        /***************************************************/

        public static PanelPlanar PanelPlanar(ICurve outline, List<ICurve> openings = null, ISurfaceProperty property = null, string name = "")
        {
            if (!outline.IIsClosed()) return null;
            List<Opening> pOpenings = openings != null ? openings.Select(o => Create.Opening(o)).ToList() : new List<Opening>();
            List<Edge> externalEdges = outline.ISubParts().Select(x => new Edge { Curve = x }).ToList();
            return PanelPlanar(externalEdges, pOpenings, property, name);
        }

        /***************************************************/

        public static PanelPlanar PanelPlanar(List<Edge> externalEdges, List<ICurve> openings = null, ISurfaceProperty property = null, string name = "")
        {
            List<Opening> pOpenings = openings != null ? openings.Select(o => Create.Opening(o)).ToList() : new List<Opening>();
            return PanelPlanar(externalEdges, pOpenings, property, name);
        }

        /***************************************************/

        public static PanelPlanar PanelPlanar(List<Edge> externalEdges, List<Opening> openings = null, ISurfaceProperty property = null, string name = "")
        {
            return new PanelPlanar
            {
                ExternalEdges = externalEdges,
                Openings = openings ?? new List<Opening>(),
                Property = property,
                Name = name
            };
        }

        /***************************************************/

        public static List<PanelPlanar> PanelPlanar(List<Polyline> outlines, ISurfaceProperty property = null, string name = "")
        {
            List<PanelPlanar> result = new List<PanelPlanar>();
            List<List<Polyline>> sortedOutlines = outlines.DistributeOutlines();
            foreach (List<Polyline> panelOutlines in sortedOutlines)
            {
                List<Edge> externalEdges = panelOutlines[0].SubParts().Select(o => new Edge { Curve = o }).ToList();
                List<Opening> openings = new List<Opening>();
                foreach (Polyline p in panelOutlines.Skip(1))
                {
                    List<Edge> openingEdges = p.SubParts().Select(o => new Edge { Curve = o }).ToList();
                    openings.Add(new Opening { Edges = openingEdges });
                }
                result.Add(new PanelPlanar { ExternalEdges = externalEdges, Openings = openings, Property = property, Name = name });
            }
            return result;
        }

        /***************************************************/

        public static List<PanelPlanar> PanelPlanar(List<ICurve> outlines, ISurfaceProperty property = null, string name = "")
        {
            List<PanelPlanar> result = new List<PanelPlanar>();
            List<List<ICurve>> sortedOutlines = outlines.DistributeOutlines();
            foreach (List<ICurve> panelOutlines in sortedOutlines)
            {
                List<Edge> externalEdges = panelOutlines[0].ISubParts().Select(o => new Edge { Curve = o }).ToList();
                List<Opening> openings = new List<Opening>();
                foreach (ICurve p in panelOutlines.Skip(1))
                {
                    List<Edge> openingEdges = p.ISubParts().Select(o => new Edge { Curve = o }).ToList();
                    openings.Add(new Opening { Edges = openingEdges });
                }
                result.Add(new PanelPlanar { ExternalEdges = externalEdges, Openings = openings, Property = property, Name = name });
            }
            return result;
        }

        /***************************************************/
    }
}

