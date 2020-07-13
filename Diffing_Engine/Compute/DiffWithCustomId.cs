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

using BH.oM.Base;
using BH.Engine;
using BH.oM.Data.Collections;
using BH.oM.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.Engine.Serialiser;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Reflection;
using System.Collections;

namespace BH.Engine.Diffing
{
    public static partial class Compute
    {
        private static Diff DiffWithCustomId(IEnumerable<IBHoMObject> pastObjects, IEnumerable<IBHoMObject> currentObjects, string customdataIdName, DiffConfig diffConfig = null)
        {
            // Here we are in the scenario where the objects do not have an HashFragment,
            // but we assume they an identifier in CustomData that let us identify the objects

            // Set configurations if diffConfig is null. Clone it for immutability in the UI.
            DiffConfig diffConfigCopy = diffConfig == null ? new DiffConfig() : diffConfig.GetShallowClone() as DiffConfig;
            if (!diffConfigCopy.PropertiesToIgnore.Contains("CustomData"))
                diffConfigCopy.PropertiesToIgnore.Add("CustomData");

            List<IBHoMObject> currentObjs = currentObjects.ToList();
            List<IBHoMObject> pastObjs = pastObjects.ToList();

            // Make dictionary with object ids to speed up the next lookups
            Dictionary<string, IBHoMObject> currObjs_dict = currentObjs.ToDictionary(obj => obj.CustomData[customdataIdName].ToString(), obj => obj);
            Dictionary<string, IBHoMObject> pastObjs_dict = pastObjs.ToDictionary(obj => obj.CustomData[customdataIdName].ToString(), obj => obj); 

            // Dispatch the objects: new, modified or deleted
            List<IBHoMObject> newObjs = new List<IBHoMObject>();
            List<IBHoMObject> modifiedObjs = new List<IBHoMObject>();
            List<IBHoMObject> deletedObjs = new List<IBHoMObject>();
            List<IBHoMObject> unChanged = new List<IBHoMObject>();

            var objModifiedProps = new Dictionary<string, Dictionary<string, Tuple<object, object>>>();

            foreach (var kv_curr in currObjs_dict)
            {
                IBHoMObject currentObj = kv_curr.Value;
                string currentObjID = kv_curr.Key;

                // Try to find an object between the pastObjs that has the same ID of the current one.
                IBHoMObject correspondingObj = null;
                pastObjs_dict.TryGetValue(kv_curr.Key, out correspondingObj);

                // If none is found, the current object is new.
                if (correspondingObj == null)
                {
                    newObjs.Add(kv_curr.Value);
                    continue;
                }

                // Otherwise, the current object existed in the past set.

                // Compute the hashes to find if they are different
                string currentHash = Compute.DiffingHash(currentObj, diffConfigCopy);
                string pastHash = Compute.DiffingHash(correspondingObj, diffConfigCopy);

                if (pastHash == currentHash)
                {
                    // It's NOT been modified
                    if (diffConfigCopy.StoreUnchangedObjects)
                        unChanged.Add(currentObj);

                    continue;
                }

                if (pastHash != currentHash)
                {
                    // It's been modified
                    modifiedObjs.Add(currentObj); 

                    if (diffConfigCopy.EnablePropertyDiffing)
                    {
                        // Determine changed properties
                        var differentProps = Query.DifferentProperties(currentObj, correspondingObj, diffConfigCopy);

                        objModifiedProps.Add(currentObjID, differentProps);
                    }

                    continue;
                }
                else
                    throw new Exception("Could not find hash information to perform Diffing on some objects.");
            }

            // If no modified property was found, set the field to null (otherwise will get empty list)
            objModifiedProps = objModifiedProps.Count == 0 ? null : objModifiedProps;

            // All ReadObjs that cannot be found by id in the previousHash of the CurrentObjs are toBeDeleted
            deletedObjs = pastObjs_dict.Keys.Except(currObjs_dict.Keys)
                .Where(k => pastObjs_dict.ContainsKey(k)).Select(k => pastObjs_dict[k]).ToList();

            return new Diff(newObjs, deletedObjs, modifiedObjs, diffConfigCopy, objModifiedProps, unChanged);
        }
    }
}
