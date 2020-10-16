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

using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Geometry;
using System;
using BH.Engine.Reflection;
using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("4.0", "BH.Engine.Geometry.Create.TubeProfile(System.Double, System.Double)")]
        [PreviousVersion("4.0", "BH.Engine.Structure.Create.TubeProfile(System.Double, System.Double)")]
        [Description("Creates a circular hollow profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("diameter")]
        [InputFromProperty("thickness")]
        [Output("tube", "The created TubeProfile.")]
        public static TubeProfile TubeProfile(double diameter, double thickness)
        {
            if (thickness >= diameter / 2)
            {
                InvalidRatioError("diameter", "thickness");
                return null;
            }

            if (diameter <= 0 || thickness <= 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = TubeProfileCurves(diameter / 2, thickness);
            return new TubeProfile(diameter, thickness, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> TubeProfileCurves(double outerRadius, double thickness)
        {
            List<ICurve> group = new List<ICurve>();
            group.AddRange(CircleProfileCurves(outerRadius));
            group.AddRange(CircleProfileCurves(outerRadius - thickness));
            return group;
        }

        /***************************************************/
    }
}
