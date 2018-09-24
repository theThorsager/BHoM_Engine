﻿using System;
using System.Collections.Generic;
using BH.oM.Environment.Elements;

using BH.oM.Base;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BuildingElement> BuildingElements(this List<IBHoMObject> bhomObjects)
        {
            List<BuildingElement> bes = new List<BuildingElement>();

            foreach(IBHoMObject obj in bhomObjects)
            {
                if (obj is BuildingElement)
                    bes.Add(obj as BuildingElement);
            }

            return bes;
        }
    }
}

