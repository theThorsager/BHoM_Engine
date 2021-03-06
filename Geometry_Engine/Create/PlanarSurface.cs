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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a PlanarSurface based on boundary curves. Only processing done by this method is checking (co)planarity and that the curves are closed. Internal edges will be assumed to be inside the External")]
        [Input("externalBoundary", "The outer boundary curve of the surface. Needs to be closed and planar")]
        [Input("internalBoundaries", "Optional internal boundary curves descibing any openings inside the external. All internal edges need to be closed and co-planar with the external edge")]
        [Output("PlanarSurface", "Planar surface corresponding to the provided edge curves")]
        public static PlanarSurface PlanarSurface(ICurve externalBoundary, List<ICurve> internalBoundaries = null)
        {
            //--------------Planar-External-Boundary-----------------------//
            if (!externalBoundary.IIsPlanar())
            {
                Reflection.Compute.RecordError("External edge curve is not planar");
                return null;
            }

            //---------------Closed-External-Boundary-----------------------//
            if (!externalBoundary.IIsClosed())
            {
                Reflection.Compute.RecordError("External edge curve is not closed");
                return null;
            }

            //--------------SelfIntersecting-External-Boundary--------------//
            if (!externalBoundary.ISubParts().Any(y => y is NurbsCurve) && externalBoundary.IIsSelfIntersecting())
            {
                Reflection.Compute.RecordError("The provided external boundary is self-intersecting.");
                return null;
            }

            internalBoundaries = internalBoundaries ?? new List<ICurve>();

            //----------------Closed-Internal-Boundaries--------------------//
            int count = internalBoundaries.Count;

            internalBoundaries = internalBoundaries.Where(x => x.IIsClosed()).ToList();

            if (internalBoundaries.Count != count)
            {
                Reflection.Compute.RecordWarning("At least one of the internal boundaries is not closed and has been ignored on creation of the planar surface.");
            }

            //---------------Coplanar-Internal-Boundaries-------------------//
            count = internalBoundaries.Count;

            Plane p = externalBoundary.IFitPlane();
            internalBoundaries = internalBoundaries.Where(x => x.IIsInPlane(p)).ToList();

            if (internalBoundaries.Count != count)
            {
                Reflection.Compute.RecordWarning("At least one of the internal boundaries is not coplanar with the external edge curve and has been ignored on creation of the planar surface.");
            }

            //--------------Unsupported-Internal-Boundaries-Warning---------//
            if (internalBoundaries.SelectMany(x => x.ISubParts()).Any(x => x is NurbsCurve || x is Ellipse))
                Reflection.Compute.RecordWarning("At least one of the internal boundaries is a NurbsCurve or Ellipse and has not been checked for validity on creation of the planar surface.");

            //--------------Self-Intersecting-Internal-Boundaries-----------//
            count = internalBoundaries.Count;

            internalBoundaries = internalBoundaries.Where(x => x.ISubParts().Any(y => y is NurbsCurve) || !x.IIsSelfIntersecting()).ToList();

            if (internalBoundaries.Count != count)
            {
                Reflection.Compute.RecordWarning("At least one of the internal boundaries is self-intersecting and has been ignored on creation of the planar surface.");
            }

            //--------------Overlapping-Internal-Boundaries-----------------//
            count = internalBoundaries.Count;

            internalBoundaries = internalBoundaries.Where(x => x.ISubParts().Any(y => y is NurbsCurve || y is Ellipse))
                                .Concat(internalBoundaries.Where(x => x.ISubParts().All(y => !(y is NurbsCurve) && !(y is Ellipse))).BooleanUnion()).ToList();

            if (internalBoundaries.Count != count)
            {
                Reflection.Compute.RecordWarning("At least one of the internalBoundaries was overlapping another one. BooleanUnion was used to resolve it.");
            }

            //--------------Unsupported-External-Boundary-------------------//
            if (externalBoundary.ISubParts().Any(x => x is NurbsCurve ||  x is Ellipse))
            {
                Reflection.Compute.RecordWarning("External boundary is a nurbs curve or Ellipse. Necessary checks to ensure validity of a planar surface based on nurbs curve cannot be run, therefore correctness of the surface boundaries is not guaranteed.");
                // External done
                return new PlanarSurface(externalBoundary, internalBoundaries);
            }

            //-------------------Internal-Boundary-Curves-------------------//
            //--------------Overlapping-External-Boundary-Curve-------------//
            for (int i = 0; i < internalBoundaries.Count; i++)
            {
                ICurve intCurve = internalBoundaries[i];
                if (intCurve.ISubParts().Any(x => x is NurbsCurve || x is Ellipse))
                    continue;

                if (externalBoundary.ICurveIntersections(intCurve).Count != 0)
                {
                    externalBoundary = externalBoundary.BooleanDifference(new List<ICurve>() { intCurve }).Single();
                    internalBoundaries.RemoveAt(i);
                    i--;
                    Reflection.Compute.RecordWarning("At least one of the internalBoundaries is intersecting the externalBoundary. BooleanDifference was used to resolve the issue.");
                }
            }

            //--------------------Internal-Boundaries-----------------------//
            //---------------Contained-By-External-Boundary-----------------//
            count = internalBoundaries.Count;

            internalBoundaries = internalBoundaries.Where(x => x.ISubParts().Any(y => y is NurbsCurve || y is Ellipse) || externalBoundary.IIsContaining(x)).ToList();

            if (internalBoundaries.Count != count)
            {
                Reflection.Compute.RecordWarning("At least one of the internalBoundaries is not contained by the externalBoundary. And have been disregarded.");
            }
            
            //------------------Return-Valid-Surface------------------------//
            return new PlanarSurface(externalBoundary, internalBoundaries);
        }

        /***************************************************/

        [Description("Distributes the edge curve and creates a set of boundary planar surfaces")]
        [Input("boundaryCurves", "Boundary curves to be used. Non-planar and non-closed curves are ignored")]
        [Output("PlanarSurface", "List of planar surfaces created")]
        public static List<PlanarSurface> PlanarSurface(List<ICurve> boundaryCurves)
        {
            List<ICurve> checkedCurves = boundaryCurves.Where(x => x.IIsClosed() && x.IIsPlanar()).ToList();
            List<List<ICurve>> distributed = Compute.DistributeOutlines(checkedCurves);

            List<PlanarSurface> surfaces = new List<PlanarSurface>();

            for (int i = 0; i < distributed.Count; i++)
            {
                PlanarSurface srf = new PlanarSurface(
                    distributed[i][0],
                    distributed[i].Skip(1).ToList()
                );

                surfaces.Add(srf);
            }

            return surfaces;
        }
        

        /***************************************************/
    }
}
