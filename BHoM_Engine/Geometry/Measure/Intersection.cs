﻿using BH.oM.Geometry;
using BHoM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Measure
    {
        /***************************************************/
        /**** Public Methods - With Line                ****/
        /***************************************************/

        public static Point GetIntersection(this Line line, Plane plane,  bool useInfiniteLine = true, double tolerance = Tolerance.Distance)
        {
            useInfiniteLine &= line.Infinite;

            Vector dir = (line.End - line.Start).GetNormalised();

            //Return null if parallel
            if (Math.Abs(dir * plane.Normal) < tolerance)
                return null;

            double t = (plane.Normal * (plane.Origin - line.Start)) / (plane.Normal * dir);

            // Return null if intersection out of segment limits
            if (!useInfiniteLine && (t < 0 || t > 1))
                return null;

            return line.Start + t * dir;
        }

        /***************************************************/

        public static Point GetIntersection(this Line line1, Line line2, bool useInfiniteLines = false, double tolerance = Tolerance.Distance)
        {
            Point pt1 = line1.Start;
            Point pt2 = line2.Start;
            Vector dir1 = (line1.End - pt1).GetNormalised();
            Vector dir2 = (line2.End - pt2).GetNormalised();

            Vector cross = Measure.GetCrossProduct(dir1, dir2);

            // Test for parallel lines
            if (cross.X < tolerance && cross.X > -tolerance && cross.Y < tolerance && cross.Y > -tolerance && cross.Z < tolerance && cross.Z > -tolerance)
            {
                if (useInfiniteLines || line1.Infinite || line2.Infinite)
                    return null;
                else if (pt1 == pt2 || pt1 == line2.End)
                    return pt1;
                else if (pt2 == line1.End || line2.End == line1.End)
                    return line1.End;
                else
                    return null;
            }

            double t = Measure.GetDotProduct(cross, Measure.GetCrossProduct(pt2 - pt1, dir2)) / Measure.GetLength(cross);

            if (useInfiniteLines)  //TODO: Need to handle the cases where one of the line is Infinite as well
                return pt1 + t * dir1;
            else if (t > -tolerance && t < Measure.GetLength(dir1) + tolerance)
                return pt1 + t * dir1;
            else return null;
        }

        /***************************************************/

        public static List<Point> GetIntersections(this List<Line> lines, bool useInfiniteLine = true, double tolerance = Tolerance.Distance)
        {
            // Get the bounding boxes
            List<BoundingBox> boxes = lines.Select(x => x.GetBounds()).ToList();

            // Get the intersections
            List<Point> intersections = new List<Point>();
            for (int i = lines.Count - 1; i >= 0; i--)     // We should use an octoTree/point matrix instead of using bounding boxes
            {
                for (int j = lines.Count - 1; j > i; j--)
                {
                    if (Verify.IsInRange(boxes[i], boxes[j]))
                    {
                        Point result = GetIntersection(lines[i], lines[j], useInfiniteLine, tolerance);
                        if (result != null)
                            intersections.Add(result);
                    }
                }
            }

            return intersections;
        }

        /***************************************************/
        /**** Public Methods - With Curve               ****/
        /***************************************************/

        public static List<Point> GetIntersections(this NurbCurve c, Plane p,  double tolerance = Tolerance.Distance)
        {
            List<double> curveParameters;
            return PlaneCurve(c, p, out curveParameters, tolerance);
        }

        /***************************************************/

        public static List<Point> PlaneCurve(this NurbCurve c, Plane p, out List<double> curveParameters, double tolerance = Tolerance.Distance)
        {
            //List<Point> result = new List<Point>();
            //int rounding = (int)Math.Log(1.0 / tolerance, 10);
            //curveParameters = new List<double>();
            //int[] sameSide = p.GetSide(c.ControlPointVector, tolerance);

            //int degree = c.GetDegree();
            //int previousSide = sameSide[0];
            //int Length = c.IsClosed() && sameSide[sameSide.Length - 1] == 0 ? sameSide.Length - 1 : sameSide.Length;

            //for (int i = 1; i < Length; i++)
            //{
            //    if (sameSide[i] != previousSide)
            //    {
            //        if (previousSide != 0)
            //        {
            //            double maxT = c.Knots[i + degree];
            //            double minT = c.Knots[i];
            //            if (i < Length - 1 && sameSide[i] == 0 && sameSide[i + 1] != 0)
            //            {
            //                maxT = c.Knots[i + degree + 1];
            //                minT = c.Knots[i + degree];
            //                i++;
            //            }
            //            result.Add(new Point(CurveParameterAtPlane(p, c, tolerance, ref minT, ref maxT, c.UnsafePointAt(minT), c.UnsafePointAt(maxT))));
            //            curveParameters.Add(Math.Round((minT + maxT) / 2, rounding));
            //        }
            //        else
            //        {
            //            result.Add(c.PointAt(c.Knots[i - 1]));
            //            curveParameters.Add(c.Knots[i - 1]);
            //        }
            //        previousSide = sameSide[i];
            //    }
            //}

            //if (sameSide[sameSide.Length - 1] == 0 && previousSide != sameSide[sameSide.Length - 1] && result.Count % 2 == 1)
            //{
            //    result.Add(c._GetEndPoint());
            //    curveParameters.Add(sameSide[sameSide.Length - 1]);
            //}

            //return result;

            throw new NotImplementedException();
        }
    }
}
