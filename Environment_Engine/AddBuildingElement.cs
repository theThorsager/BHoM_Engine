using BH.oM.Environmental.Interface;
using BH.oM.Environmental.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Architecture.Elements;
using System.Collections.Generic;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Building AddBuildingElement(this Building building, BuildingElement buildingElement)
        {
            Building aBuilding = building.GetShallowClone() as Building;
            aBuilding.BuildingElements = new List<BuildingElement>(building.BuildingElements);

            if (buildingElement == null)
                return null;

            aBuilding.BuildingElements.Add(buildingElement);

            //TODO: Add missing BuildingElementProperties to building
            //TODO: Add missing Level to project

            return aBuilding;
        }

        /***************************************************/
    }
}