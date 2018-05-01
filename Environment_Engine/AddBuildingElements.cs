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

        public static Building AddBuildingElements(this Building building, IEnumerable<BuildingElement> buildingElements)
        {
            Building aBuilding = building.GetShallowClone() as Building;
            aBuilding.BuildingElements = new List<BuildingElement>(building.BuildingElements);

            if (buildingElements == null)
                return null;

            aBuilding.BuildingElements.AddRange(buildingElements);

            //TODO: Add missing BuildingElementProperties to building
            //TODO: Add missing Level to project

            return aBuilding;
        }

        /***************************************************/
    }
}
