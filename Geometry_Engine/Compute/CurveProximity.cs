/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point[] CurveProximity(this Line curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            if (curve1.CurveIntersections(curve2).Count > 0)
                return new Point[] {curve1.CurveIntersections(curve2)[0],curve1.CurveIntersections(curve2)[0] };
            Point min1 = new Point();
            Point min2 = new Point();
            min1 = curve1.Start;
            min2 = curve2.Start;
            if(curve1.End.Distance(curve2)<min1.Distance(curve2))
            {
                min1 = curve1.End;
            }
            if(curve2.End.Distance(curve1)<min2.Distance(curve1))
            {
                min2 = curve2.End;
            }
            if (min2.Distance(curve1) < min1.Distance(curve2))
            {
                min1 = curve1.ClosestPoint(min2);
            }
            else
                min2 = curve2.ClosestPoint(min1);
            if (curve1.IsCoplanar(curve2))
            {
                return new Point[] { min1, min2 };
            }

            double[] t = curve1.SkewLineProximity(curve2);
            double t1 = Math.Max(Math.Min(t[0], 1), 0);
            double t2 = Math.Max(Math.Min(t[1], 1), 0);
            Vector e1 = curve1.End - curve1.Start;
            Vector e2 = curve2.End - curve2.Start;
            if ((curve1.Start + e1 * t1).Distance(curve2.Start + e2 * t2) < min1.Distance(min2))
                return new Point[] { curve1.Start + e1 * t1, curve2.Start + e2 * t2 };
            else
                return new Point[] { min1, min2 };
        }

        /***************************************************/

        public static Point[] CurveProximity(this Line curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            if (curve1.CurveIntersections(curve2).Count > 0)
                return new Point[] { curve1.CurveIntersections(curve2)[0], curve1.CurveIntersections(curve2)[0] };
            double distance1 = Math.Min(curve2.EndPoint().Distance(curve1), curve2.StartPoint().Distance(curve1));
            double distance2 = Math.Min(curve1.End.Distance(curve2), curve1.Start.Distance(curve2));
            Point min1 = curve1.Start;
            Point min2 = curve2.StartPoint();
            if (min1.Distance(curve2) > curve1.End.Distance(curve2))
                min1 = curve1.End;
            if (min2.Distance(curve1) > curve2.EndPoint().Distance(curve1))
                min2 = curve2.EndPoint();
            if (min1.Distance(curve2) < min2.Distance(curve1))
                min2 = curve2.ClosestPoint(min1);
            else
                min1 = curve1.ClosestPoint(min2);
            if (curve1.IIsCoplanar(curve2))
                return new Point[] {min1,min2 };
            Point start, end, binSearch;
            Point result1, result2;
            if (distance1 < distance2)
            {
                Arc temp = curve2;
                start = curve2.StartPoint();
                end = curve2.EndPoint();
                while ((start - end).Length() > tolerance * tolerance)
                {
                    binSearch = temp.PointAtParameter(0.5);
                    if (end.Distance(curve1) > start.Distance(curve1))
                        end = binSearch;
                    else
                        start = binSearch;
                    temp = temp.Trim(start, end);
                }
                if (start.Distance(curve1) < end.Distance(curve1))
                    result1 = start;
                else
                    result1 = end;
                result2 = curve1.ClosestPoint(result1);
            }
            else
            {
                start = curve1.Start;
                end = curve1.End;
                while ((start - end).Length() > tolerance * tolerance)
                {
                    binSearch = start + ((end - start) / 2);
                    if (start.Distance(curve2) > end.Distance(curve2))
                        start = binSearch;
                    else
                        end = binSearch;
                }
                if (start.Distance(curve2) < end.Distance(curve2))
                    result1 = start;
                else
                    result1 = end;
                result2 = curve2.ClosestPoint(result1);
            }
            if (min1.Distance(min2) > result2.Distance(result1))
                return new Point[] { result1, result2 };
            return new Point[] { min1, min2 };
        }

        /***************************************************/

        public static Point[] CurveProximity(this Line curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            Point[] list = new Point[4];
            for(double i=0;i<1;i+=0.25)
            {
                list[(int)(4 * i)] = curve2.PointAtParameter(i);
            }
            Arc temp1 = Create.Arc(list[0], list[1], list[2]);
            Arc temp2 = Create.Arc(list[2], list[3], list[0]);
            Point[] result1 = curve1.CurveProximity(temp1);
            Point[] result2 = curve1.CurveProximity(temp2);
            if (result1[0].Distance(result1[1]) < result2[0].Distance(result2[1]))
                return result1;
            else
                return result2;
        }

        /***************************************************/

        public static Point[] CurveProximity(this Line curve1, PolyCurve curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.CurveProximity(curve1);
        }

        /***************************************************/

        public static Point[] CurveProximity(this Line curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.CurveProximity(curve1);
        }

        /***************************************************/

        public static Point[] CurveProximity(this Arc curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.CurveProximity(curve1);
        }

        /***************************************************/

        public static Point[] CurveProximity(this Arc curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            if (curve1.CurveIntersections(curve2).Count > 0)
                return new Point[] { curve1.CurveIntersections(curve2)[0], curve1.CurveIntersections(curve2)[1] };
            Point min1, min2;
            double distance1 = Math.Min(curve2.EndPoint().Distance(curve1), curve2.StartPoint().Distance(curve1));
            double distance2 = Math.Min(curve1.EndPoint().Distance(curve2), curve1.StartPoint().Distance(curve2));
            min1 = curve1.StartPoint();
            min2 = curve2.StartPoint();
            if (curve1.EndPoint().Distance(curve2) < min1.Distance(curve2))
                min1 = curve1.EndPoint();
            if (curve2.EndPoint().Distance(curve1) < min2.Distance(curve1))
                min2 = curve2.EndPoint();
            if (min1.Distance(curve2) < min2.Distance(curve1))
                min2 = curve2.ClosestPoint(min1);
            else
                min1 = curve1.ClosestPoint(min2);
            Arc tmp, tmp2;
            tmp = curve1;
            tmp2 = curve2;
            if (distance2 < distance1)
            {
                tmp = curve2;
                tmp2 = curve1;
            }
            Point start = tmp2.StartPoint();
            Point end = tmp2.EndPoint();
            Point binSearch = new Point();
            while ((start - end).Length() > tolerance * tolerance)
            {
                binSearch = tmp2.PointAtParameter(0.5);
                if (start.Distance(tmp) > end.Distance(tmp))
                    start = binSearch;
                else
                    end = binSearch;
                tmp2 = tmp2.Trim(start, end);
            }
            if (start.Distance(tmp) > end.Distance(tmp))
                end = tmp.ClosestPoint(start);
            else
                start = tmp.ClosestPoint(end);
            if (start.Distance(end) > min1.Distance(min2))
                return new Point[] { min1, min2 };
            else
            {
                if (start.IsOnCurve(curve1))
                    return new Point[] { start, end };
                else
                    return new Point[] { end, start };

            }
        }

        /***************************************************/

        public static Point[] CurveProximity(this Arc curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            if (curve1.CurveIntersections(curve2).Count > 0)
                return new Point[] { curve1.CurveIntersections(curve2)[0], curve1.CurveIntersections(curve2)[1] };
            Point[] crclpts = new Point[4];
            for (double i = 0; i < 1; i += 0.25)
            {
                crclpts[(int)(4 * i)] = curve2.PointAtParameter(i);
            }
            Arc tmp = Create.Arc(crclpts[0], crclpts[1], crclpts[2]);
            Arc tmp2 = Create.Arc(crclpts[2], crclpts[3], crclpts[0]);
            Point[] result1 = tmp.CurveProximity(curve1);
            Point[] result2 = tmp2.CurveProximity(curve1);
            if (result1[0].Distance(result1[1]) < result2[0].Distance(result2[1]))
                return new Point[] { result1[0], result1[1] };
            else
                return new Point[] { result2[0], result2[1] };
        }

        /***************************************************/

        public static Point[] CurveProximity(this Arc curve1, PolyCurve curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.CurveProximity(curve1);
        }

        /***************************************************/

        public static Point[] CurveProximity(this Arc curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.CurveProximity(curve1);
        }

        /***************************************************/

        public static Point[] CurveProximity(this Circle curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.CurveProximity(curve1);
        }

        /***************************************************/

        public static Point[] CurveProximity(this Circle curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.CurveProximity(curve1);
        }

        /***************************************************/

        public static Point[] CurveProximity(this Circle curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            if (curve1.CurveIntersections(curve2).Count > 0)
                return new Point[] { curve1.CurveIntersections(curve2)[0], curve1.CurveIntersections(curve2)[1] };
            Point[] crclpts = new Point[4];
            for (double i = 0; i < 1; i += 0.25)
            {
                crclpts[(int)(4 * i)] = curve2.PointAtParameter(i);
            }
            Arc tmp = Create.Arc(crclpts[0], crclpts[1], crclpts[2]);
            Arc tmp2 = Create.Arc(crclpts[2], crclpts[3], crclpts[0]);
            Point[] result1 = tmp.CurveProximity(curve1);
            Point[] result2 = tmp2.CurveProximity(curve1);
            if (result1[0].Distance(result1[1]) < result2[0].Distance(result2[1]))
                return new Point[] { result1[0], result1[1] };
            else
                return new Point[] { result2[0], result2[1] };
        }

        /***************************************************/

        public static Point[] CurveProximity(this Circle curve1, PolyCurve curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.CurveProximity(curve1);
        }

        /***************************************************/

        public static Point[] CurveProximity(this Circle curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.CurveProximity(curve1);
        }

        /***************************************************/

        public static Point[] CurveProximity(this PolyCurve curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            Point[] result = curve2.ICurveProximity(curve1.Curves[0]);
            Point[] cp = new Point[2];
            for (int i = 1; i < curve1.Curves.Count; i++)
            {
                cp = curve2.ICurveProximity(curve1.Curves[i]);
                if (cp[0].Distance(cp[1])< result[0].Distance(result[1]))
                {
                    result = curve2.ICurveProximity(curve1.Curves[i]);
                }
            }
            return result;
        }

        /***************************************************/

        public static Point[] CurveProximity(this PolyCurve curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            Point[] result = curve2.ICurveProximity(curve1.Curves[0]);
            Point[] cp = new Point[2];
            for (int i = 1; i < curve1.Curves.Count; i++)
            {
                cp = curve2.ICurveProximity(curve1.Curves[i]);
                if (cp[0].Distance(cp[1]) < result[0].Distance(result[1]))
                {
                    result = curve2.ICurveProximity(curve1.Curves[i]);
                }
            }
            return result;
        }

        /***************************************************/

        public static Point[] CurveProximity(this PolyCurve curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            Point[] result = curve2.ICurveProximity(curve1.Curves[0]);
            Point[] cp = new Point[2];
            for (int i = 1; i < curve1.Curves.Count; i++)
            {
                cp = curve2.ICurveProximity(curve1.Curves[i]);
                if (cp[0].Distance(cp[1]) < result[0].Distance(result[1]))
                {
                    result = cp;
                }
            }
            return result;
        }

        /***************************************************/

        public static Point[] CurveProximity(this PolyCurve curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve2.ControlPoints.Count - 1; i++)
                temp.Add(Create.Line(curve2.ControlPoints[i], curve2.ControlPoints[i + 1]));
            if (curve2.IsClosed())
                temp.Add(Create.Line(curve2.ControlPoints[curve2.ControlPoints.Count - 1], curve2.ControlPoints[0]));
            Point[] result = temp[0].CurveProximity(curve1);
            Point[] cp = new Point[2];
            for (int i = 1; i < temp.Count; i++)
            {
                cp = temp[i].CurveProximity(curve1);
                if (cp[0].Distance(cp[1]) < result[0].Distance(result[1]))
                {
                    result = cp;
                }
            }
            return result;
        }

        /***************************************************/

        public static Point[] CurveProximity(this PolyCurve curve1, PolyCurve curve2, double tolerance = Tolerance.Distance)
        {
            Point[] result = curve2.Curves[0].ICurveProximity(curve1);
            Point[] temp = new Point[2];
            for (int i = 0; i < curve2.Curves.Count; i++)
            {
                temp = curve2.Curves[i].ICurveProximity(curve1);
                    if (temp[0].Distance(temp[1]) < result[0].Distance(result[1]))
                    {
                        result = temp;
                    }
            }
            return result;
        }

        /***************************************************/

        public static Point[] CurveProximity(this Polyline curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve1.ControlPoints.Count - 1; i++)
                temp.Add(Create.Line(curve1.ControlPoints[i], curve1.ControlPoints[i + 1]));
            Point[] result = curve2.CurveProximity(temp[0]);
            Point[] cp = new Point[2];
            for (int i = 1; i < temp.Count; i++)
            {
                cp = curve2.CurveProximity(temp[i]);
                if (cp[0].Distance(cp[1])<result[0].Distance(result[1]))
                {
                    result = cp;
                }
            }
            return result;
        }

        /***************************************************/

        public static Point[] CurveProximity(this Polyline curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve1.ControlPoints.Count - 1; i++)
                temp.Add(Create.Line(curve1.ControlPoints[i], curve1.ControlPoints[i + 1]));
            Point[] result = temp[0].ICurveProximity(curve2);
            Point[] cp  = new Point[2];
            for (int i = 1; i < temp.Count; i++)
            {
                cp = temp[i].ICurveProximity(curve2);
                if (cp[0].Distance(cp[1]) < result[0].Distance(result[1]))
                    result = cp;
            }
            return result;
        }

        /***************************************************/

        public static Point[] CurveProximity(this Polyline curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve1.ControlPoints.Count - 1; i++)
                temp.Add(Create.Line(curve1.ControlPoints[i], curve1.ControlPoints[i + 1]));
            Point[] result = temp[0].ICurveProximity(curve2);
            Point[] cp = new Point[2];
            for (int i = 1; i < temp.Count; i++)
            {
                cp = temp[i].ICurveProximity(curve2);
                if (cp[0].Distance(cp[1]) < result[0].Distance(result[1]))
                    result = cp;
            }
            return result;
        }

        /***************************************************/

        public static Point[] CurveProximity(this Polyline curve1, PolyCurve curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.CurveProximity(curve1);
        }

        /***************************************************/

        public static Point[] CurveProximity(this Polyline curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve1.ControlPoints.Count - 1; i++)
                temp.Add(Create.Line(curve1.ControlPoints[i], curve1.ControlPoints[i + 1]));
            Point[] result = temp[0].ICurveProximity(curve2);
            Point[] cp = new Point[2];
            for (int i = 1; i < temp.Count; i++)
            {
                cp = temp[i].ICurveProximity(curve2);
                if (cp[0].Distance(cp[1]) < result[0].Distance(result[1]))
                    result = cp;
            }
            return result;
        }

        /***************************************************/

        [NotImplemented]
        public static Point[] CurveProximity(this ICurve curve1, Ellipse curve2, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static Point[] CurveProximity(this ICurve curve1, NurbsCurve curve2, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point[] ICurveProximity(this ICurve curve1, ICurve curve2, double tolerance = Tolerance.Distance)
        {
            return CurveProximity(curve1 as dynamic, curve2 as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double[] SkewLineProximity(this Line line1, Line line2, double angleTolerance = Tolerance.Angle)
        {
            Vector v1 = line1.End - line1.Start;
            Vector v2 = line2.End - line2.Start;
            Vector v1N = v1.Normalise();
            Vector v2N = v2.Normalise();

            if (v1N == null || v2N == null || 1 - Math.Abs(v1N.DotProduct(v2N)) <= angleTolerance)
                return null;

            Point p1 = line1.Start;
            Point p2 = line2.Start;

            Vector cp = v1.CrossProduct(v2);
            Vector n1 = v1.CrossProduct(-cp);
            Vector n2 = v2.CrossProduct(cp);

            double t1 = (p2 - p1) * n2 / (v1 * n2);
            double t2 = (p1 - p2) * n1 / (v2 * n1);

            return new double[] { t1, t2 };
        }

        /***************************************************/
    }
}