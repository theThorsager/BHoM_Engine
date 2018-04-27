using BH.oM.Environmental.Interface;
using BH.oM.Environmental.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Architecture.Elements;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BuildingElement SetAdjacentSpaces(this BuildingElement buildingElement, System.Guid spaceGuid_1, System.Guid spaceGuid_2)
        {
            BuildingElement aBuildingElement = buildingElement.GetShallowClone() as BuildingElement;
            aBuildingElement.AdjacentSpaces = new System.Collections.Generic.List<System.Guid>();
            aBuildingElement.AdjacentSpaces.Add(spaceGuid_1);
            aBuildingElement.AdjacentSpaces.Add(spaceGuid_2);
            return aBuildingElement;
        }

        public static BuildingElement SetAdjacentSpaces(this BuildingElement buildingElement, System.Guid spaceGuid)
        {
            BuildingElement aBuildingElement = buildingElement.GetShallowClone() as BuildingElement;
            aBuildingElement.AdjacentSpaces = new System.Collections.Generic.List<System.Guid>();
            aBuildingElement.AdjacentSpaces.Add(spaceGuid);
            return aBuildingElement;
        }

        public static BuildingElement SetAdjacentSpaces(this BuildingElement buildingElement_Destination, BuildingElement buildingElement_Source)
        {
            BuildingElement aBuildingElement = buildingElement_Destination.GetShallowClone() as BuildingElement;
            if (buildingElement_Source.AdjacentSpaces == null)
                aBuildingElement.AdjacentSpaces = null;
            else
                aBuildingElement.AdjacentSpaces = new System.Collections.Generic.List<System.Guid>(buildingElement_Source.AdjacentSpaces);

            return aBuildingElement;
        }
    }
}

