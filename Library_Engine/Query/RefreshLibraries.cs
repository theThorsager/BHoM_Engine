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
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base;
using System.IO;
using System.Linq;
using BH.oM.Data.Collections;
using BH.Engine.Data;
using BH.oM.Data.Library;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void RefreshLibraries()
        {
            m_libraryPaths = new Dictionary<string, List<string>>();
            m_libraryStrings = new Dictionary<string, string[]>();
            m_datasets = new Dictionary<string, Dataset>();
            m_deserialisationEvents = new Dictionary<string, List<Tuple<oM.Reflection.Debugging.EventType, string>>>();
            m_dbTree = new Tree<string>();
            GetPathsAndLoadLibraries();
        }
    }
}

