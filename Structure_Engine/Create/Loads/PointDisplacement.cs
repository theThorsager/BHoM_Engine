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

using BH.oM.Geometry;
using BH.oM.Structure.Loads;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a point displacement to be applied to Nodes.")]
        [InputFromProperty("loadcase")]
        [InputFromProperty("group", "Objects")]
        [InputFromProperty("translation")]
        [InputFromProperty("rotation")]
        [InputFromProperty("axis")]
        [Input("name", "The name of the created load.")]
        [Output("ptDisp", "The created PointDisplacement.")]
        public static PointDisplacement PointDisplacement(Loadcase loadcase, BHoMGroup<Node> group, Vector translation = null, Vector rotation = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            if (translation == null && rotation == null)
                throw new ArgumentException("Point displacement requires either the translation or the rotation vector to be defined");

            return new PointDisplacement
            {
                Loadcase = loadcase,
                Objects = group,
                Translation = translation == null ? new Vector() : translation,
                Rotation = rotation == null ? new Vector() : rotation,
                Axis = axis,
                Name = name
            };

        }

        /***************************************************/

        [Description("Creates a point displacement to be applied to Nodes.")]
        [InputFromProperty("loadcase")]
        [Input("objects", "The collection of Nodes the load should be applied to.")]
        [InputFromProperty("translation")]
        [InputFromProperty("rotation")]
        [InputFromProperty("axis")]
        [Input("name", "The name of the created load.")]
        [Output("ptDisp", "The created PointDisplacement.")]
        public static PointDisplacement PointDisplacement(Loadcase loadcase, IEnumerable<Node> objects, Vector translation = null, Vector rotation = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            return PointDisplacement(loadcase, new BHoMGroup<Node>() { Elements = objects.ToList() }, translation, rotation, axis, name);
        }

        /***************************************************/

    }
}

