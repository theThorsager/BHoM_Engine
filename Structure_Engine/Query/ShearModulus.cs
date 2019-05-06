﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Structure.MaterialFragments;
using BH.oM.Physical.Materials;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Shear modulus of the isotropic material fragment. Evaluated based on Youngs modulus and Poissons ratio")]
        [Output("G", "Shear modulus of the material fragment")]
        public static double ShearModulus(this IIsotropic materialFragment)
        {
            return materialFragment.YoungsModulus / (2 * (1 + materialFragment.PoissonsRatio));
        }

        /***************************************************/

        [Description("Shear modulus of the material. Can be either a double or a vector depending on the material type")]
        [Output("G", "Shear modulus of the material. Double for Isotropic material, Vector for orthotropic")]
        public static object ShearModulus(this Material material)
        {
            return material.StructuralMaterialFragment()?.IShearModulus();
        }

        /***************************************************/

        [Description("Shear modulus for a isotropic material")]
        [Output("G", "Shear modulus of the material. 0 if material does not contain any structural material fragments")]
        public static double ShearModulusIsotropic(this Material material)
        {
            return (double)material.IsotropicMaterialFragment()?.ShearModulus();
        }

        /***************************************************/

        [Description("Shear modulus for a orthotropic material")]
        [Output("G", "Shear modulus of the material. null if material does not contain any structural material fragments")]
        public static Vector ShearModulusOrthotropic(this Material material)
        {
            return material.OrthotropicMaterialFragment()?.ShearModulus;
        }


        /***************************************************/
        /**** Public Methods  - Interace                ****/
        /***************************************************/

        [Description("Gets Shear modulus from the material fragment")]
        public static object IShearModulus(this IStructuralMaterial materialFragment)
        {
            return ShearModulus(materialFragment as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Private extension method added to alow for dynamic casting")]
        private static Vector ShearModulus(this IOrthotropic materialFragment)
        {
            return materialFragment.ShearModulus;
        }

        /***************************************************/
    }
}