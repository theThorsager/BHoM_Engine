using BH.oM.Environmental.Interface;
using BH.oM.Environmental.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Architecture.Elements;
using System.Collections.Generic;

using System;
using System.Linq;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Building RemoveBuildingElements(this Building building, IEnumerable<BuildingElement> buildingElements)
        {
            if (buildingElements == null)
                return building;

            Building aBuilding = building.GetShallowClone() as Building;

            aBuilding.BuildingElements = new List<BuildingElement>(building.BuildingElements);

            foreach(BuildingElement aBuildingElement in buildingElements)
            {
                BuildingElement aBuildingElement_Temp = aBuilding.BuildingElements.Find(x => x.BHoM_Guid == aBuildingElement.BHoM_Guid);
                if (aBuildingElement_Temp != null)
                {
                    foreach (Guid g in aBuildingElement_Temp.AdjacentSpaces)
                    {
                        var s = aBuilding.Spaces.Where(x => x.BHoM_Guid == g).FirstOrDefault();
                        if (s != null)
                        {
                            s.BuildingElements.Remove(aBuildingElement_Temp);
                        }
                    }

                    aBuilding.BuildingElements.Remove(aBuildingElement_Temp);
                }
            }

            //TODO: Test for all cases

            return aBuilding;
        }

        /***************************************************/
    }
}
