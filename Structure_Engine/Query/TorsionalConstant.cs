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

using System;
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        //public static double TorsionalConstantThinWalled(ShapeType shape, double totalDepth, double totalWidth, double b1, double b2, double tf1, double tf2, double tw)
        //{
        //    switch (shape)
        //    {
        //        case ShapeType.ISection:
        //        case ShapeType.Channel:
        //        case ShapeType.Zed:
        //            return (b1 * Math.Pow(tf1, 3) + b2 * Math.Pow(tf2, 3) + (totalDepth - tf1) * Math.Pow(tw, 3)) / 3;
        //        case ShapeType.Tee:
        //        case ShapeType.Angle:
        //            return totalWidth * Math.Pow(tf1, 3) + totalDepth * Math.Pow(tw, 3);
        //        case ShapeType.Circle:
        //            return Math.PI * Math.Pow(totalDepth, 4) / 2;
        //        case ShapeType.Box:
        //            return 2 * tf1 * tw * Math.Pow(totalWidth - tw, 2) * Math.Pow(totalDepth - tf1, 2) /
        //                (totalWidth * tw + totalDepth * tf1 - Math.Pow(tw, 2) - Math.Pow(tf1, 2));
        //        case ShapeType.Tube:
        //            return Math.PI * (Math.Pow(totalDepth, 4) - Math.Pow(totalDepth - tw, 4)) / 2;
        //        case ShapeType.Rectangle:
        //            if (Math.Abs(totalDepth - totalWidth) < Tolerance.Distance)
        //                return 2.25 * Math.Pow(totalDepth, 4);
        //            else
        //            {
        //                double a = Math.Max(totalDepth, totalWidth);
        //                double b = Math.Min(totalDepth, totalWidth);
        //                return a * Math.Pow(b, 3) * (16 / 3 - 3.36 * b / a * (1 - Math.Pow(b, 4) / (12 * Math.Pow(a, 4))));
        //            }
        //        default:
        //            return 0;
        //    }
        //}


        /***************************************************/

        [Description("Calcualtes the Torsinal constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this CircleProfile profile)
        {
            return Math.PI * Math.Pow(profile.Diameter, 4) / 32;
        }

        /***************************************************/

        [Description("Calcualtes the Torsinal constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this TubeProfile profile)
        {
            return Math.PI * (Math.Pow(profile.Diameter, 4) - Math.Pow(profile.Diameter - 2* profile.Thickness, 4)) / 32;
        }

        /***************************************************/

        [Description("Calcualtes the Torsinal constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this FabricatedBoxProfile profile)
        {
            double tf1 = profile.TopFlangeThickness; //TODO: Allow for varying plate thickness
            double tw = profile.WebThickness;
            double width = profile.Width;
            double height = profile.Height;


            return 2 * tf1 * tw * Math.Pow(width - tw, 2) * Math.Pow(height - tf1, 2) /
                        (width * tw + height * tf1 - Math.Pow(tw, 2) - Math.Pow(tf1, 2));
        }

        /***************************************************/

        [Description("Calcualtes the Torsinal constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this BoxProfile profile)
        {
            double tf1 = profile.Thickness;
            double tw = profile.Thickness;
            double width = profile.Width;
            double height = profile.Height;



            return 2 * tf1 * tw * Math.Pow(width - tw, 2) * Math.Pow(height - tf1, 2) /
                        (width * tw + height * tf1 - Math.Pow(tw, 2) - Math.Pow(tf1, 2));
        }

        /***************************************************/

        [Description("Calcualtes the Torsinal constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this FabricatedISectionProfile profile)
        {
            double b1 = profile.TopFlangeWidth;
            double b2 = profile.BotFlangeWidth;
            double height = profile.Height;
            double tf1 = profile.TopFlangeThickness;
            double tf2 = profile.BotFlangeThickness;
            double tw = profile.WebThickness;

            return (b1 * Math.Pow(tf1, 3) + b2 * Math.Pow(tf2, 3) + (height - (tf1 + tf2) / 2) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        [Description("Calcualtes the Torsinal constant for the profile. Note that this is not the polar moment of inertia.\n" +
                      "Formulae taken from https://orangebook.arcelormittal.com/explanatory-notes/long-products/section-properties/")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this ISectionProfile profile)
        {
            double b = profile.Width;
            double height = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;
            double r = profile.RootRadius;

            double alpha = -0.042 + 0.2204 * tw / tf + 0.1355 * r / tf - 0.0865 * (r * tw) / Math.Pow(tf, 2) - 0.0725 * Math.Pow(tw / tf, 2);
            double D = (Math.Pow(tf + r, 2) + (r + 0.25 * tw) * tw) / (2 * r + tf);

            //note the 'd's after the numbers needed to force values being recognised as doubles. Without 2/3 and 1/3 evaluates to 0 from integer division.
            return 2d / 3d * b * Math.Pow(tf, 3) + 1d / 3d * (height - 2 * tf) * Math.Pow(tw, 3) + 2 * alpha * Math.Pow(D, 4) - 0.420 * Math.Pow(tf, 4);
        }


        /***************************************************/

        [Description("Calcualtes the Torsinal constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this ChannelProfile profile)
        {
            double b = profile.FlangeWidth;
            double height = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;

            return (2 * (b - tw / 2) * Math.Pow(tf, 3) + (height - tf) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        [Description("Calcualtes the Torsinal constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this ZSectionProfile profile)
        {
            double b1 = profile.FlangeWidth;
            double b2 = profile.FlangeWidth;
            double height = profile.Height;
            double tf1 = profile.FlangeThickness;
            double tf2 = profile.FlangeThickness;
            double tw = profile.WebThickness;

            return (b1 * Math.Pow(tf1, 3) + b2 * Math.Pow(tf2, 3) + (height - tf1) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        [Description("Calcualtes the Torsinal constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this TSectionProfile profile)
        {
            double totalWidth = profile.Width;
            double totalDepth = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;

            return (totalWidth * Math.Pow(tf, 3) + (totalDepth - tf / 2) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        [Description("Calcualtes the Torsinal constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this GeneralisedTSectionProfile profile)
        {

            bool leftOutstand = profile.LeftOutstandWidth > 0 && profile.LeftOutstandThickness > 0;
            bool rightOustand = profile.RightOutstandWidth > 0 && profile.RightOutstandThickness > 0;

            if (!leftOutstand && !rightOustand)
            {
                //No outstands => Rectangle

                double a = Math.Max(profile.Height, profile.WebThickness) / 2;
                double b = Math.Min(profile.Height, profile.WebThickness) / 2;
                return a * Math.Pow(b, 3) * (16 / 3 - 3.36 * b / a * (1 - Math.Pow(b, 4) / (12 * Math.Pow(a, 4))));
            }

            if (Math.Abs(profile.RightOutstandThickness - profile.LeftOutstandThickness) < Tolerance.Distance
                && Math.Abs(profile.RightOutstandWidth - profile.LeftOutstandWidth) < Tolerance.Distance)
            {
                //Symmetric T
                double totalWidth = profile.RightOutstandWidth * 2 + profile.WebThickness;
                double totalDepth = profile.Height;
                double tf = profile.RightOutstandThickness;
                double tw = profile.WebThickness;

                return (totalWidth * Math.Pow(tf, 3) + (totalDepth - tf / 2) * Math.Pow(tw, 3)) / 3;
            }


            if (leftOutstand && !rightOustand || !leftOutstand && rightOustand)
            {
                //One outstand => angle
                double totalWidth = (leftOutstand ? profile.LeftOutstandWidth : profile.RightOutstandWidth) + profile.WebThickness;
                double totalDepth = profile.Height;
                double tf = leftOutstand ? profile.LeftOutstandThickness : profile.RightOutstandThickness;
                double tw = profile.WebThickness;

                return ((totalWidth - tw / 2) * Math.Pow(tf, 3) + (totalDepth - tf / 2) * Math.Pow(tw, 3)) / 3;
            }


            Reflection.Compute.RecordWarning("Can only calculate torsional constant of symmetric T sections or angles");
            return 0;
            
        }

        /***************************************************/

        [Description("Calcualtes the Torsinal constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this AngleProfile profile)
        {
            double totalWidth = profile.Width;
            double totalDepth = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;

            return ((totalWidth - tw / 2) * Math.Pow(tf, 3) + (totalDepth - tf / 2) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        [Description("Calcualtes the Torsinal constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this RectangleProfile profile)
        {
            if (Math.Abs(profile.Height - profile.Width) < Tolerance.Distance)
                return 2.25 * Math.Pow(profile.Height/2, 4);
            else
            {
                double a = Math.Max(profile.Height, profile.Width)/2;
                double b = Math.Min(profile.Height, profile.Width)/2;
                return a * Math.Pow(b, 3) * (16 / 3 - 3.36 * b / a * (1 - Math.Pow(b, 4) / (12 * Math.Pow(a, 4))));
            }
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Calcualtes the Torsinal constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double ITorsionalConstant(this IProfile profile)
        {
            return TorsionalConstant(profile as dynamic);
        }

        /***************************************************/
        /**** Private Methods - fall back               ****/
        /***************************************************/

        private static double TorsionalConstant(this IProfile profile)
        {
            Reflection.Compute.RecordWarning("Can not calculate Tosional constants for profiles of type " + profile.GetType().Name + ". Returned value will be 0.");
            return 0; //Return 0 for not specifically implemented ones
        }

        /***************************************************/
    }
}

