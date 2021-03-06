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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Fragments;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        [Deprecated("3.0", "Deprecated to remove name input", null, "RadiationFragment(lightingRadiation, occupantRadiation, equipmentRadiation)")]
        public static RadiationFragment RadiationFragment(string name = "", double lightingRadiation = 0.0, double occupantRadiation = 0.0, double equipmentRadiation = 0.0)
        {
            return Create.RadiationFragment(lightingRadiation, occupantRadiation, equipmentRadiation);
        }

        [Description("Returns a Coefficient Fragment object")]
        [Input("lightingRadiation", "The lighting radiation, default 0.0")]
        [Input("occupantRadiation", "The occupant radiation, default 0.0")]
        [Input("equipmentRadiation", "The equipment radiation, default 0.0")]
        [Output("radiationFragment", "An Environment Coefficient Fragment object - this can be added to any Environment object")]
        [Deprecated("3.0", "Deprecated in favour of default create components produced by BHoM")]
        public static RadiationFragment RadiationFragment(double lightingRadiation = 0.0, double occupantRadiation = 0.0, double equipmentRadiation = 0.0)
        {
            return new RadiationFragment
            {
                LightingRadiation = lightingRadiation,
                OccupantRadiation = occupantRadiation,
                EquipmentRadiation = equipmentRadiation,
            };
        }
    }
}

