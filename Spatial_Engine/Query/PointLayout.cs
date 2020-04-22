﻿/*
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
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Spatial.Layouts;
using BH.Engine.Geometry;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Queries the points from the Layout based on the layout and a region curve associated with it.")]
        [Input("layout2D", "The layout object to query the points from.")]
        [Input("hostRegionCurve", "The region curve of the objects associated with the layout.")]
        [Output("points", "Point layout generated by the layout objects and region curve.")]
        public static List<Point> PointLayout(this ExplicitLayout layout2D, ICurve hostRegionCurve)
        {
            return layout2D.Points.ToList();
        }

        /***************************************************/

        [Description("Queries the points from the Layout based on the layout and a region curve associated with it.")]
        [Input("layout2D", "The layout object to query the points from.")]
        [Input("hostRegionCurve", "The region curve of the objects associated with the layout.")]
        [Output("points", "Point layout generated by the layout objects and region curve.")]
        public static List<Point> PointLayout(this PerimiterLayout layout2D, ICurve hostRegionCurve)
        {
            List<ICurve> subCurves = hostRegionCurve.ISubParts().ToList();
            if (layout2D.EnforceDiscontinuityPoints && subCurves.Count != 1)
            {
                List<Point> pts = hostRegionCurve.IDiscontinuityPoints().CullDuplicates();
                int remainingPoints = layout2D.NumberOfPoints - pts.Count;
                if (remainingPoints > 0)
                {
                    
                    List<int> divisions = DistributeDivisions(subCurves, remainingPoints);

                    for (int i = 0; i < subCurves.Count; i++)
                    {
                        if (divisions[i] == 0)
                            continue;

                        //Sample points always includes start and end (which are removed later) and gives back 1 more point than divisions.
                        //To make sure point(s) on the middle of the are extracted, division is increased by 1, which should return 2 more points than divisions asked for.
                        int div = divisions[i] + 1; 

                        List<Point> subPts = subCurves[i].SamplePoints(div);

                        //Remove points at start/end
                        subPts.RemoveAt(subPts.Count - 1);
                        subPts.RemoveAt(0);
                        pts.AddRange(subPts);
                    }

                }
                pts = pts.ISortAlongCurve(hostRegionCurve);
                return pts;
            }
            else
            {
                int divisions = layout2D.NumberOfPoints;
                bool closed = hostRegionCurve.IIsClosed();

                if (!closed)
                    divisions--;

                List<Point> pts = hostRegionCurve.SamplePoints(divisions);

                //Remove duplicate endpoint
                if (closed)
                    pts.RemoveAt(pts.Count - 1);

                return pts;
            }
        }

        /***************************************************/

        [Description("Queries the points from the Layout based on the layout and a region curve associated with it.")]
        [Input("layout2D", "The layout object to query the points from.")]
        [Input("hostRegionCurve", "The region curve of the objects associated with the layout.")]
        [Output("points", "Point layout generated by the layout objects and region curve.")]
        public static List<Point> PointLayout(this LinearLayout layout2D, ICurve hostRegionCurve)
        {
            Point centre = ReferencePoint(hostRegionCurve, layout2D.ReferencePoint);
            centre = centre + layout2D.Offset * Vector.ZAxis.CrossProduct(layout2D.Direction);
            Line axis = new Line { Start = centre, End = centre + layout2D.Direction, Infinite = true };

            List<Line> distributionLines = IntersectionLines(hostRegionCurve, axis);

            if (distributionLines.Count == 0)
            {
                Engine.Reflection.Compute.RecordError("Count not find extents of distribution for the layout.");
                return new List<Point>();
            }

            //SamplePoints adds extra point at start/end. Hence subtracting count here to make sure correct number is extracted.
            List<int> divisions = DistributeDivisions(distributionLines, layout2D.NumberOfPoints - distributionLines.Count);

            List<Point> result = new List<Point>();

            for (int i = 0; i < distributionLines.Count; i++)
            {
                List<Point> divPts = distributionLines[i].SamplePoints(divisions[i]);
                result.AddRange(divPts);
            }
            return result;
        }

        /***************************************************/

        [Description("Queries the points from the Layout based on the layout and a region curve associated with it.")]
        [Input("layout2D", "The layout object to query the points from.")]
        [Input("hostRegionCurve", "The region curve of the objects associated with the layout.")]
        [Output("points", "Point layout generated by the layout objects and region curve.")]
        public static List<Point> PointLayout(this MultiLinearLayout layout2D, ICurve hostRegionCurve)
        {
            Point centre = ReferencePoint(hostRegionCurve, layout2D.ReferencePoint);

            Vector offsetDir = Vector.ZAxis.CrossProduct(layout2D.Direction);

            centre = centre + layout2D.Offset* offsetDir;

            int remainingPoints = layout2D.NumberOfPoints;

            List<Point> result = new List<Point>();

            while (remainingPoints > 0)
            {
                Line axis = new Line { Start = centre, End = centre + layout2D.Direction, Infinite = true };
                List<Line> distributionLines = IntersectionLines(hostRegionCurve, axis);

                if (distributionLines.Count == 0)
                {
                    Engine.Reflection.Compute.RecordError("Could not generate distribution lines for the reinforcement. The number of points might not fit in the region curve. The resulting number of points might be different from the number requested.");
                    remainingPoints = 0;
                }

                //If number of point remaining is less or equal to the number of distribution lines,
                //add a point in the middle of the longest lines, until points have run out
                if (remainingPoints <= distributionLines.Count)
                {
                    distributionLines = distributionLines.OrderBy(x => x.ILength()).ToList();
                    int i = 0;
                    while (remainingPoints > 0 && i < distributionLines.Count)
                    {
                        result.Add(distributionLines[i].PointAtParameter(0.5));
                        i++;
                        remainingPoints--;
                    }
                    break;
                }

                int layerDivs = 0;
                for (int i = 0; i < distributionLines.Count; i++)
                {
                    int divs = (int)Math.Floor(distributionLines[i].ILength() / layout2D.MinimumSpacing);
                    layerDivs += Math.Max(divs, 2);
                }

                layerDivs = Math.Min(layerDivs, remainingPoints);

                //SamplePoints adds extra point at start/end. Hence subtracting count here to make sure correct number is extracted.
                List<int> divisions = DistributeDivisions(distributionLines, layerDivs - distributionLines.Count);

                for (int i = 0; i < distributionLines.Count; i++)
                {
                    List<Point> divPts = distributionLines[i].SamplePoints(divisions[i]);
                    result.AddRange(divPts);
                }
                remainingPoints -= layerDivs;
                centre += offsetDir * layout2D.MinimumSpacing;
            }

            return result;
        }


        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Queries the points from the Layout based on the layout and a region curve associated with it.")]
        [Input("layout2D", "The layout object to query the points from.")]
        [Input("hostRegionCurve", "The region curve of the objects associated with the layout.")]
        [Output("points", "Point layout generated by the layout objects and region curve.")]
        public static List<Point> IPointLayout(this ILayout2D layout2D, ICurve hostRegionCurve)
        {
            return PointLayout(layout2D as dynamic, hostRegionCurve);
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static List<Point> PointLayout(this ILayout2D layout2D, ICurve hostRegionCurve)
        {
            Reflection.Compute.RecordError("PointLayout for " + layout2D.GetType().Name + " is not implemented.");
            return new List<Point>();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<int> DistributeDivisions(IEnumerable<ICurve> curves, int nbPoints)
        {
            List<double> lengths = curves.Select(x => x.ILength()).ToList();
            double fullLength = lengths.Sum();

            List<int> divs = new List<int>();
            List<double> remaining = new List<double>();

            //Check how many division points to extract from each curve, based on length ratio of the curve in relation to the length of all curves
            for (int i = 0; i < lengths.Count; i++)
            {
                double pointRatio = lengths[i] / fullLength * nbPoints;
                int fullCurveDivs = (int)Math.Floor(pointRatio);
                double remainder = pointRatio - fullCurveDivs;

                divs.Add(fullCurveDivs);
                remaining.Add(remainder);
            }

            int remPoints = nbPoints - divs.Sum();

            //Add points to the curve that has the largest reminder after the ratio extraction
            while (remPoints > 0)
            {
                int maxIndex = remaining.IndexOf(remaining.Max());
                divs[maxIndex]++;
                remaining[maxIndex] = 0;
                remPoints--;
            }

            return divs;

        }

        /***************************************************/

        private static Point ReferencePoint(ICurve hostElementCurve, ReferencePoint referencePoint)
        {
            if (referencePoint == oM.Spatial.Layouts.ReferencePoint.Centroid)
            {
                return hostElementCurve.ICentroid();
            }
            BoundingBox bounds = hostElementCurve.IBounds();
            
            switch (referencePoint)
            {
                case oM.Spatial.Layouts.ReferencePoint.BottomLeft:
                    return new Point { X = bounds.Min.X, Y = bounds.Min.Y };
                case oM.Spatial.Layouts.ReferencePoint.BottomCenter:
                    return new Point { X = (bounds.Min.X + bounds.Max.X) / 2, Y = bounds.Min.Y };
                case oM.Spatial.Layouts.ReferencePoint.BottomRight:
                    return new Point { X = bounds.Max.X, Y = bounds.Min.Y };
                case oM.Spatial.Layouts.ReferencePoint.MiddleLeft:
                    return new Point { X = bounds.Min.X, Y = bounds.Min.Y };
                case oM.Spatial.Layouts.ReferencePoint.MiddleCenter:
                    return bounds.Centre();
                case oM.Spatial.Layouts.ReferencePoint.MiddleRight:
                    return new Point { X = bounds.Max.X, Y = (bounds.Min.Y + bounds.Max.Y) / 2 };
                case oM.Spatial.Layouts.ReferencePoint.TopLeft:
                    return new Point { X = bounds.Min.X, Y = bounds.Max.Y };
                case oM.Spatial.Layouts.ReferencePoint.TopCenter:
                    return new Point { X = (bounds.Min.X + bounds.Max.X) / 2, Y = bounds.Max.Y };
                case oM.Spatial.Layouts.ReferencePoint.TopRight:
                    return new Point { X = bounds.Max.X, Y = bounds.Max.Y };
                default:
                    return hostElementCurve.ICentroid();
            }
        }

        /***************************************************/

        private static List<Line> IntersectionLines(this ICurve hostRegionCurve, Line axis, double tolerance = Tolerance.Distance)
        {
            List<Point> intPts = hostRegionCurve.ILineIntersections(axis, true);
            intPts = intPts.CullDuplicates(tolerance);
            intPts = intPts.SortCollinear(tolerance);

            List<Line> result = new List<Line>();
            for (int i = 0; i < intPts.Count - 1; i++)
            {
                if (hostRegionCurve.IIsContaining(new List<Point> { intPts.Skip(i).Take(2).Average() }, true, tolerance))
                    result.Add(new Line { Start = intPts[i], End = intPts[i + 1] });
            }

            //Ensure a continous line is merged into one.
            result = Engine.Geometry.Compute.Join(result).Select(x => new Line { Start = x.IStartPoint(), End = x.IEndPoint(), Infinite = false }).ToList();

            return result;
        }

        /***************************************************/
    }
}
