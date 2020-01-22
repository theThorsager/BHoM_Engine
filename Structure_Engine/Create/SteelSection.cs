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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Geometry;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Reflection;
using BH.oM.Reflection.Attributes;
using System.Linq;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a steel I-section from input dimensions. Please note that all units are in S.I., that is meters [m]")]
        [Input("height", "Full height of the section [m]")]
        [Input("webThickness", "Thickness of the web [m]")]
        [Input("flangeWidth", "Width of the top and bottom flange [m]")]
        [Input("flangeThickness", "Thickness of the top and bottom flange [m]")]
        [Input("rootRadius", "Optional fillet radius between inner face of flange and face of web [m]")]
        [Input("toeRadius", "Optional fillet radius at the outer edge of the flange [m]")]
        [Input("material", "Steel material to be used on the section. If null a default material will be extracted from the database")]
        [Input("name", "Name of the steel section.")]
        [Output("section", "The created steel I-section")]
        public static SteelSection SteelISection(double height, double webThickness, double flangeWidth, double flangeThickness, double rootRadius = 0, double toeRadius = 0, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.ISectionProfile(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius), material, name);
        }

        /***************************************************/

        [Description("Creates a fabricated steel I-section from input dimensions. Please note that all units are in S.I., that is meters [m]")]
        [Input("height", "Full height of the section [m]")]
        [Input("webThickness", "Thickness of the web [m]")]
        [Input("topFlangeWidth", "Width of the top flange [m]")]
        [Input("topFlangeThickness", "Thickness of the top flange [m]")]
        [Input("botFlangeWidth", "Width of the bottom flange [m]")]
        [Input("botFlangeThickness", "Thickness of the bottom flange [m]")]
        [Input("weldSize", "Optional fillet weld size between web and flanges. Meassured as the distance between intersection of web and flange perpendicular to the edge of the weld [m]")]
        [Input("material", "Steel material to be used on the section. If null a default material will be extracted from the database")]
        [Input("name", "Name of the steel section.")]
        [Output("section", "The created fabricated steel I-section")]
        public static SteelSection SteelFabricatedISection(double height, double webThickness, double topFlangeWidth, double topFlangeThickness, double botFlangeWidth, double botFlangeThickness,  double weldSize, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.FabricatedISectionProfile(height, topFlangeWidth, botFlangeWidth, webThickness, topFlangeThickness, botFlangeThickness, weldSize), material, name);
        }

        /***************************************************/

        [Description("Creates a steel box-section from input dimensions. Please note that all units are in S.I., that is meters [m]")]
        [Input("height", "Full height of the section [m]")]
        [Input("width", "Full width of the section [m]")]
        [Input("thickness", "Thickness of the webs and flanges [m]")]
        [Input("innerRadius", "Optional inner corner radius. Commonly set equal to the thickness [m]")]
        [Input("outerRadius", "Optional outer corner radius. Commonly set to 1.5-2 times the thickness [m]")]
        [Input("material", "Steel material to be used on the section. If null a default material will be extracted from the database")]
        [Input("name", "Name of the steel section.")]
        [Output("section", "The created steel box-section")]
        public static SteelSection SteelBoxSection(double height, double width, double thickness, double innerRadius = 0, double outerRadius = 0, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.BoxProfile(height, width, thickness, outerRadius, innerRadius), material, name);
        }

        /***************************************************/

        [Description("Creates a fabricated steel box-section from input dimensions. Please note that all units are in S.I., that is meters [m]")]
        [Input("height", "Full height of the section [m]")]
        [Input("width", "Full width of the section [m]")]
        [Input("webThickness", "Thickness of the webs [m]")]
        [Input("flangeThickness", "Thickness of the flanges [m]")]
        [Input("weldSize", "Optional fillet weld size between inside of web and flanges. Meassured as the distance between intersection of web and flange perpendicular to the edge of the weld [m]")]
        [Input("material", "Steel material to be used on the section. If null a default material will be extracted from the database")]
        [Input("name", "Name of the steel section.")]
        [Output("section", "The created fabricated steel box-section")]
        public static SteelSection FabricatedSteelBoxSection(double height, double width, double webThickness, double flangeThickness, double weldSize, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.FabricatedBoxProfile(height, width, webThickness, flangeThickness, flangeThickness, weldSize), material, name);
        }

        /***************************************************/

        [Description("Creates a circular hollow steel section from input dimensions. Please note that all units are in S.I., that is meters [m]")]
        [Input("diameter", "Outer diameter of the section [m]")]
        [Input("thickness", "Plate thickness of the section [m]")]
        [Input("material", "Steel material to be used on the section. If null a default material will be extracted from the database")]
        [Input("name", "Name of the steel section.")]
        [Output("section", "The created circular hollow steel section")]
        public static SteelSection SteelTubeSection(double diameter, double thickness, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.TubeProfile(diameter, thickness), material, name);
        }

        /***************************************************/

        [Description("Creates a rectangular solid steel section from input dimensions. Please note that all units are in S.I., that is meters [m]")]
        [Input("height", "Height of the section [m]")]
        [Input("width", "Width of the section [m]")]
        [Input("cornerRadius", "Optional corner radius for the section [m]")]
        [Input("material", "Steel material to be used on the section. If null a default material will be extracted from the database")]
        [Input("name", "Name of the steel section.")]
        [Output("section", "The created rectangular solid steel section")]
        public static SteelSection SteelRectangleSection(double height, double width, double cornerRadius = 0, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.RectangleProfile(height, width, cornerRadius), material, name);
        }

        /***************************************************/

        [Description("Creates a circular solid steel section from input dimensions. Please note that all units are in S.I., that is meters [m]")]
        [Input("diameter", "Diameter of the section [m]")]
        [Input("material", "Steel material to be used on the section. If null a default material will be extracted from the database")]
        [Input("name", "Name of the steel section.")]
        [Output("section", "The created circular solid steel section")]
        public static SteelSection SteelCircularSection(double diameter, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.CircleProfile(diameter), material, name);
        }

        /***************************************************/

        [Description("Creates a steel T-section from input dimensions. Please note that all units are in S.I., that is meters [m]")]
        [Input("height", "Full height of the section [m]")]
        [Input("webThickness", "Thickness of the web [m]")]
        [Input("flangeWidth", "Width of the top and bottom flange [m]")]
        [Input("flangeThickness", "Thickness of the top and bottom flange [m]")]
        [Input("rootRadius", "Optional fillet radius between inner face of flange and face of web [m]")]
        [Input("toeRadius", "Optional fillet radius at the outer edge of the flange [m]")]
        [Input("material", "Steel material to be used on the section. If null a default material will be extracted from the database")]
        [Input("name", "Name of the steel section.")]
        [Output("section", "The created steel T-section")]
        public static SteelSection SteelTSection(double height, double webThickness, double flangeWidth, double flangeThickness,  double rootRadius = 0, double toeRadius = 0, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.TSectionProfile(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius), material, name);

        }

        /***************************************************/

        [Description("Creates a steel L-section from input dimensions. Please note that all units are in S.I., that is meters [m]")]
        [Input("height", "Full height of the section [m]")]
        [Input("webThickness", "Thickness of the web [m]")]
        [Input("flangeWidth", "Width of the top and bottom flange [m]")]
        [Input("flangeThickness", "Thickness of the top and bottom flange [m]")]
        [Input("rootRadius", "Optional fillet radius between inner face of flange and face of web [m]")]
        [Input("toeRadius", "Optional fillet radius at the outer edge of the flange [m]")]
        [Input("material", "Steel material to be used on the section. If null a default material will be extracted from the database")]
        [Input("name", "Name of the steel section.")]
        [Output("section", "The created steel L-section")]
        public static SteelSection SteelAngleSection(double height, double webThickness, double width, double flangeThickness, double rootRadius = 0, double toeRadius = 0, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.AngleProfile(height, width, webThickness, flangeThickness, rootRadius, toeRadius), material, name);
        }

        /***************************************************/

        [Description("Creates a steel freeform section based on edge curves. Please note that this type of section generally will have less support in adapters. If the type of section being created can be achieved by any other profile, aim use them that instead.")]
        [Input("edges", "Edges defining the section. Should consist of closed curve(s) in the global xy-plane")]
        [Input("material", "Steel material to be used on the section. If null a default material will be extracted from the database")]
        [Input("name", "Name of the steel section.")]
        [Output("section", "The created free form steel section")]
        public static SteelSection SteelFreeFormSection(List<ICurve> edges, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Geometry.Create.FreeFormProfile(edges), material, name);
        }

        /***************************************************/

        [Description("Generates a steel section based on a Profile and a material. \n This is the main create method for steel sections, responsible for calculating section constants etc. and is being called from all other create methods for steel sections")]
        [Input("profile", "The section profile the steel section. All section constants are derived based on the dimensions of this")]
        [Input("material", "Steel material to be used on the section. If null a default material will be extracted from the database")]
        [Input("name", "Name of the steel section. If null or empty the name of the profile will be used")]
        [Output("section", "The created steel section")]
        public static SteelSection SteelSectionFromProfile(IProfile profile, Steel material = null, string name = "")
        {
            //Run pre-process for section create. Calculates all section constants and checks name of profile
            var preProcessValues = PreProcessSectionCreate(name, profile);
            name = preProcessValues.Item1;
            profile = preProcessValues.Item2;
            Dictionary<string,object> constants= preProcessValues.Item3;

            SteelSection section = new SteelSection(profile,
                (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"], (double)constants["Wely"],
                (double)constants["Welz"], (double)constants["Wply"], (double)constants["Wplz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
                (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

            //Postprocess section. Sets default name if null, and grabs default material for section if noting is provided
            return PostProcessSectionCreate(section, name, material, MaterialType.Steel);

        }

        /***************************************************/
    }
}
