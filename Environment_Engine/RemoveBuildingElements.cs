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
                    //Remove the BE from all the spaces
                    for(int x = 0; x < aBuilding.Spaces.Count; x++)
                    {
                        for(int y = 0; y < aBuilding.Spaces[x].BuildingElements.Count; y++)
                        {
                            if (aBuilding.Spaces[x].BuildingElements[y].BHoM_Guid == aBuildingElement.BHoM_Guid)
                                aBuilding.Spaces[x].BuildingElements.Remove(aBuildingElement);
                        }
                    }
                    //This is a bit of a slower method but is the only one that worked of all the ones tested...

                    aBuilding.BuildingElements.Remove(aBuildingElement_Temp);
                }
            }

            //TODO: Test for all cases

            return aBuilding;
        }

        /***************************************************/
    }
}
