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

using BH.Engine.Geometry;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [NotImplemented]
        [Description("Queries the centre of weight for a IElement1Ds ICurve representation.")]
        [Input("element1D", "The IElement1D with the geometry to get the centre of weight of. The IElement1D will be considered homogeneous.")]
        [Output("centroid", "The Point at the centre of weight for the homogeneous geometrical representation of the IElement1D.")]
        public static Point Centroid(this IElement1D element1D)
        {
            //TODO: find a proper centre of weight of a curve (not an average of control points)
            throw new NotImplementedException();
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [Description("Queries the centre of area for a IElement2Ds surface representation. For an IElement2D with homogeneous material and thickness this will also be the centre of weight.")]
        [Input("element2D", "The IElement2D with the geometry to get the centre of area of.")]
        [Output("centroid", "The Point at the centre of area for the homogeneous geometrical representation of the IElement2D.")]
        public static Point Centroid(this IElement2D element2D)
        {
            Point tmp = Geometry.Query.Centroid(element2D.OutlineCurve());
            double area = Geometry.Query.Area(element2D.OutlineCurve());

            double x = tmp.X * area;
            double y = tmp.Y * area;
            double z = tmp.Z * area;


            List<PolyCurve> openings = Geometry.Compute.BooleanUnion(element2D.InternalOutlineCurves());

            foreach (ICurve o in openings)
            {
                Point oTmp = Geometry.Query.ICentroid(o);
                double oArea = o.IArea();
                x -= oTmp.X * oArea;
                y -= oTmp.Y * oArea;
                z -= oTmp.Z * oArea;
                area -= oArea;
            }
            
            return new Point { X = x / area, Y = y / area, Z = z / area };
        }

        /******************************************/
    }
}
