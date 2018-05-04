using BH.oM.Environmental.Interface;
using BH.oM.Environmental.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Architecture.Elements;

using System.Linq;
using System;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Building RemoveBuildingElement(this Building building, BuildingElement buildingElement)
        {
            if (buildingElement == null)
                return building;

            Building aBuilding = building.GetShallowClone() as Building;

            aBuilding.BuildingElements = new System.Collections.Generic.List<BuildingElement>(building.BuildingElements);

            BuildingElement aBuildingElement = aBuilding.BuildingElements.Find(x => x.BHoM_Guid == buildingElement.BHoM_Guid);
            if(aBuildingElement != null)
            {
                foreach(Guid g in aBuildingElement.AdjacentSpaces)
                {
                    var s = aBuilding.Spaces.Where(x => x.BHoM_Guid == g).FirstOrDefault();
                    if(s != null)
                    {
                        s.BuildingElements.Remove(aBuildingElement);
                    }
                }
                
                aBuilding.BuildingElements.Remove(aBuildingElement);
            }

            //TODO: Test for all cases

            return aBuilding;
        }

        /***************************************************/
    }
}