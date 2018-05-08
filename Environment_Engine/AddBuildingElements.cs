using System;
using System.Linq;

using BH.oM.Environmental.Interface;
using BH.oM.Environmental.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Architecture.Elements;
using System.Collections.Generic;

using BH.Engine.Geometry;

using ClipperLib;

namespace BH.Engine.Environment
{
    using Polygon = List<IntPoint>;
    using Polygons = List<List<IntPoint>>;

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

            foreach(BuildingElement be in buildingElements)
            {
                //Change the GUID to a unique one
                Guid g = Guid.NewGuid();
                while (aBuilding.BuildingElements.Where(x => x.BHoM_Guid == g).FirstOrDefault() != null)
                    g = Guid.NewGuid(); //Ensure a unique ID is used that is not in use by other elements

                be.BHoM_Guid = g;
            }

            foreach(BuildingElement be in buildingElements)
            {
                foreach(Guid g in be.AdjacentSpaces)
                {
                    var s = aBuilding.Spaces.Where(x => x.BHoM_Guid == g).FirstOrDefault();
                    if (s != null)
                    {
                        aBuilding.Spaces.Where(x => x.BHoM_Guid == g).FirstOrDefault().BuildingElements.Add(be);
                    }

                }

                aBuilding.BuildingElements.Add(be);
            }

            //TODO: Test for all cases
            //TODO: Add missing BuildingElementProperties to building
            //TODO: Add missing Level to project

            return aBuilding;
        }

        /***************************************************/
    }
}
