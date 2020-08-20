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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.Engine.Geometry;
using BH.Engine.Spatial;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Get the Vector basis system descibring the local axis orientation of the Panel in the global coordinate system where the z-axis is the normal of the panel and the x- and y-axes are the directions of the local in-plane axes.")]
        [Input("panel", "The Panel to extract the local orientation from from.")]
        [Output("orienation", "The local orientation of the Panel as a vector Basis.")]
        public static Basis LocalOrientation(this Panel panel)
        {
            Vector normal = Engine.Spatial.Query.Normal(panel);

            Vector localX, localY;

            if (normal.IsParallel(Vector.XAxis) == 0)
            {
                //Normal not parallel to global X
                localX = Vector.XAxis.Project(new Plane { Normal = normal }).Normalise();
                localX = localX.Rotate(panel.OrientationAngle, normal);
                localY = normal.CrossProduct(localX);
            }
            else
            {
                //Normal is parallel to global x
                localY = Vector.YAxis.Project(new Plane { Normal = normal }).Normalise();
                localY = localY.Rotate(panel.OrientationAngle, normal);
                localX = localY.CrossProduct(normal);
            }

            return new Basis(localX, localY, normal);

        }

        /***************************************************/
    }
}

