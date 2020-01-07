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
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.Elements;
using BH.oM.Base;

using BH.Engine.Base;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Physical.FramingProperties;
using System;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the normal of a FramingElement, which would be the Y-axis in the local coordinate syetem")]
        [Input("framingElement", "A FramingElement")]
        [Output("normal", "The FramingElements normal vector")]
        public static Vector Normal(this IFramingElement framingElement)
        {
            double orientationAngle;
            if (!(framingElement.Property is ConstantFramingProperty))
            {
                Reflection.Compute.RecordWarning("No ConstantFramingProperty found, OrientationAngle set as 0");
                orientationAngle = 0;
            } else
            {
                orientationAngle = (framingElement.Property as ConstantFramingProperty).OrientationAngle;
            }

            if (framingElement.Location.IIsLinear())
            {
                Point p1 = framingElement.Location.IStartPoint();
                Point p2 = framingElement.Location.IEndPoint();

                Vector tan = (p2 - p1).Normalise();
                Vector normal;

                if (!IsVertical(p1, p2))
                {
                    normal = Vector.ZAxis;
                    normal = (normal - tan.DotProduct(normal) * tan).Normalise();
                }
                else
                {
                    Vector locY = Vector.YAxis;
                    locY = (locY - tan.DotProduct(locY) * tan).Normalise();
                    normal = tan.CrossProduct(locY);
                }

                return normal.Rotate(orientationAngle, tan);
            } else if (framingElement.Location.IIsPlanar())
            {
                Vector tan = framingElement.Location.IStartDir();   // Is this how we should define it?
                //Vector tan = framingElement.Location.IEndPoint() - framingElement.Location.IStartPoint();
                return framingElement.Location.IFitPlane().Normal.Rotate(orientationAngle, tan);    // The normal could potentially flip by moving some control points
            }
            else
            {
                Engine.Reflection.Compute.RecordError("The normal for non-planar framing elements is not implemented");
                return null;
            }
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool IsVertical(Point p1, Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;

            return Math.Sqrt(dx * dx + dy * dy) < 0.0001;
        }

        /***************************************************/
    }
}