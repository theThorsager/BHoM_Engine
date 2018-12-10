﻿using BH.oM.Common;
using BH.oM.Structure.Elements;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static List<IElement1D> OutlineElements1D(this Opening opening)
        {
            return new List<IElement1D>(opening.Edges);
        }

        /***************************************************/

        public static List<IElement1D> OutlineElements1D(this PanelPlanar panelPlanar)
        {
            return new List<IElement1D>(panelPlanar.ExternalEdges);
        }

        /***************************************************/
    }
}