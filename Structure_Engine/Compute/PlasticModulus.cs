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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Reflection;

namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates the PlasticModulus [Wpl] for an enclosed region")]
        [Input("pLine", "Polyline, should have the upper side along the x-axis and the rest of the lines should be definable as a function of x apart for vertical segments. \n" +
                        "The last LineSegment should be the upper one, use WetBlanketInterpretation() to convert a collection of regions to compliant form")]
        [Input("curves", "The true curves of the section where counter-clockwise curves are positive area and clockwise ones are negative. /n" +
                         "Should be the curves whose Polyline version was provided for WetBlanketInterpetation()")]
        [Input("trueArea", "The true area of the region")]
        [Output("plasticModulus", "The plasticModulus for the region")]
        public static double PlasticModulus(this Polyline pLine, IEnumerable<ICurve> curves = null, double trueArea = double.NaN)
        {
            double area = BH.Engine.Geometry.Compute.IIntegrateRegion(pLine, 0);   //Should be calculated here for consistency

            if (curves == null)
            {
                curves = new List<ICurve>() { pLine };
                if (double.IsNaN(trueArea))
                    trueArea = area;
            } else if (double.IsNaN(trueArea))
                trueArea = curves.Sum(x => x.IIntegrateRegion(0));

            double halfTrueArea = trueArea * 0.5;

            double neutralAxis = PlasticNeutralAxis(pLine, area * 0.5);

            if (double.IsNaN(neutralAxis))
            {
                Engine.Reflection.Compute.RecordError("NeutralAxis not found, PlasticModulus set to 0");
                return 0;
            }

            // Create the half regions to find their pivot point
            List<ICurve> splitCurve = ISplitAtX(curves, neutralAxis).ToList();

            double plasticModulus = 0;
            double upperArea = 0;
            double temp, d = 0;
            foreach (ICurve curve in splitCurve)    // ISplitAtX maintains the curve direction, and hence holes remain holes and areas remain areas
            {
                if (curve.IPointAtParameter(0.5).X < neutralAxis)
                    plasticModulus -= curve.Close(out temp).IIntegrateRegion(1);
                else
                {
                    ICurve closed = curve.Close(out temp);
                    upperArea += closed.IIntegrateRegion(0);
                    plasticModulus += closed.IIntegrateRegion(1);
                    d += temp;
                }
            }

            if (Math.Abs(1 - halfTrueArea / upperArea) > Tolerance.Distance)
            {
                // Nudges the solution to be correct, will give exact result if the neutralAxis and the diff is somewhere with constant thickness, 
                // otherwise very close to it.
                double diff = (halfTrueArea - upperArea) / d;   // missing area divided by sectionthickness = width of the missing area
                double add = (3 * d * ( Math.Pow(neutralAxis - diff, 2) -  Math.Pow(neutralAxis, 2)) / 6); //SpecialCase of IntegrateRegion(1)
                plasticModulus -= 2 * add;
            }
            return Math.Abs(plasticModulus);
        }

        /***************************************************/

        [Description("Closes a curve with a line if open")]
        private static ICurve Close(this ICurve curve, out double d, double tol = Tolerance.Distance)
        {
            Point start = curve.IStartPoint();
            Point end = curve.IEndPoint();

            if (start.SquareDistance(end) > tol * tol)
            {
                d = end.Y - start.Y;    // This should be able to be negative
                return new PolyCurve() { Curves = new List<ICurve>() { curve, new Line() { Start = end, End = start } } };
            }

            d = 0;
            return curve.IClone();
        }

        /***************************************************/

        private static double PlasticNeutralAxis(this Polyline pLine, double halfArea)
        {
            double partialArea = 0;

            for (int i = 0; i < pLine.ControlPoints.Count - 2; i++)
            {
                Point first = pLine.ControlPoints[i];
                Point second = pLine.ControlPoints[i + 1];

                double currentLineArea = Engine.Geometry.Compute.IntSurfLine(first, second, 0);
                double currentCapArea = Engine.Geometry.Compute.IntSurfLine(second, new Point() { X = second.X }, 0);

                if (partialArea + currentLineArea + currentCapArea < halfArea)
                {
                    partialArea += currentLineArea;
                }
                else
                {
                    double x = first.X;
                    double z = second.X;

                    double zx = z - x;
                    if (Math.Abs(zx) < Tolerance.Distance)  //Vertical Line
                        return x;

                    double y = first.Y;
                    double u = second.Y;

                    double uy = u - y;
                    if (Math.Abs(uy) < Tolerance.Distance)  //Horizontal Line
                        return ((halfArea - partialArea) / -y);

                    double a = -(uy) / (2 * (zx));
                    double b = ((uy) * x - y * (zx)) / (zx);
                    double c = -Math.Pow(x, 2) * (uy) / (2 * (zx)) - (halfArea - partialArea);

                    // quadratic formula
                    double sqrt = Math.Sqrt(Math.Pow(b, 2) - 4 * a * c);
                    double x1 = (-b + sqrt) / (2 * a);
                    double x2 = (-b - sqrt) / (2 * a);

                    bool between1 = (x1 >= x - Tolerance.MicroDistance && x1 < z + Tolerance.MicroDistance);
                    bool between2 = (x2 >= x - Tolerance.MicroDistance && x2 < z + Tolerance.MicroDistance);

                    if (between1 && between2)           //TODO test if its always x1 or x2 or whatever (99% that it's x1)
                    {
                        Engine.Reflection.Compute.RecordWarning("two solutions: x1 = " + x1 + " x2 = " + x2);
                        return double.NaN;
                    }
                    else if (between1)
                        return x1;
                    else if (between2)
                        return x2;
                    else
                    {
                        Engine.Reflection.Compute.RecordWarning($"both solutions invalid, x1 = {x1} x2 = {x2}");
                        return double.NaN;
                    }
                }
            }
            return double.NaN;
        }

        /***************************************************/
        /****     Private SplitAtX                       ***/
        /***************************************************/

        private static IEnumerable<ICurve> ISplitAtX(IEnumerable<ICurve> curves, double x, double tol = Tolerance.Distance)
        {
            // Assumes 2D
            List<ICurve> results = new List<ICurve>();
            foreach (ICurve curve in curves)
            {
                results.AddRange(SplitAtX(curve as dynamic, x, tol));
            }
            return results;
        }

        /***************************************************/

        private static IEnumerable<ICurve> SplitAtX(PolyCurve polyCurve, double x, double tol = Tolerance.Distance)
        {
            // Assumes 2D
            List<ICurve> results = new List<ICurve>();

            if (polyCurve.Curves.Count == 1 && polyCurve.Curves[0] is Circle)
                return SplitAtX(polyCurve.Curves[0] as Circle, x, tol);

            List<ICurve> temp;
            PolyCurve current = new PolyCurve();
            for (int i = 0; i < polyCurve.Curves.Count; i++)
            {
                temp = SplitAtX(polyCurve.Curves[i] as dynamic, x, tol);
                if (temp.Count > 1)
                {
                    current.Curves.Add(temp[0]);
                    results.Add(current);
                    if (temp.Count != 2)
                        results.AddRange(temp.GetRange(1, temp.Count - 2));
                    current = new PolyCurve();
                    current.Curves.Add(temp.Last());
                }
                else
                    current.Curves.AddRange(temp);

            }
            results.Add(current);
            if (results.Count > 1 && polyCurve.IsClosed() && Math.Abs(polyCurve.StartPoint().X - x) > tol)
            {
                results.Add(new PolyCurve() { Curves = results.Last().ISubParts().Concat(results.First().ISubParts()).ToList() });
                results.RemoveAt(results.Count - 2);
                results.RemoveAt(0);
            }
            return results;
        }

        /***************************************************/

        private static IEnumerable<ICurve> SplitAtX(Polyline polyline, double x, double tol = Tolerance.Distance)
        {
            // Assumes 2D
            List<ICurve> results = new List<ICurve>();

            int lastSplit = 0;
            Point ptOn = null;
            for (int i = 0; i < polyline.ControlPoints.Count - 1; i++)
            {
                Point one = polyline.ControlPoints[i];
                Point two = polyline.ControlPoints[i + 1];

                if (Math.Abs(two.X - x) < tol ) //On Point
                {
                    List<Point> control = new List<Point>(polyline.ControlPoints.GetRange(lastSplit, i + 2 - lastSplit));
                    if (ptOn != null)
                        control[0] = ptOn;
                    results.Add(new Polyline() { ControlPoints = control });
                    ptOn = null;
                    lastSplit = i + 1;
                }
                else if (one.X < x - tol && two.X > x ||    // On Line
                           one.X > x + tol && two.X < x)
                {
                    List<Point> control = new List<Point>(polyline.ControlPoints.GetRange(lastSplit, i + 1 - lastSplit));
                    if (ptOn != null)
                        control[0] = ptOn;
                    
                    ptOn = Geometry.Query.PointAtX(one, two, x);
                    control.Add(ptOn);
                    results.Add(new Polyline() { ControlPoints = control });
                    lastSplit = i;
                }
            }

            if (ptOn != null || lastSplit != polyline.ControlPoints.Count - 2) // Finish Curve
            {
                List<Point> control = new List<Point>(polyline.ControlPoints.GetRange(lastSplit, polyline.ControlPoints.Count - lastSplit));
                if (ptOn != null)
                    control[0] = ptOn;
                results.Add(new Polyline() { ControlPoints = control });

                if (polyline.IsClosed(tol))     // Close curve
                {
                    (results[0] as Polyline).ControlPoints.RemoveAt(0);
                    results.Add(new Polyline() { ControlPoints = results.Last().IControlPoints().Concat(results.First().IControlPoints()).ToList() });
                    results.RemoveAt(results.Count - 2);
                    results.RemoveAt(0);
                }
            }

            return results;
        }

        /***************************************************/

        private static IEnumerable<ICurve> SplitAtX(Arc arc, double x, double tol = Tolerance.Distance)
        {
            // Assumes 2D
            List<ICurve> results = new List<ICurve>();

            if (IntersectsArc(arc, x))
            {
                Point centre = arc.CoordinateSystem.Origin;
                results.AddRange(SplitAtX(new Circle() { Centre = centre, Radius = arc.Radius }, x, tol));
                Point start = arc.StartPoint();
                Point end = arc.EndPoint();
                double startAngle = Vector.XAxis.SignedAngle(start - centre, Vector.ZAxis);
                double endAngle = Vector.XAxis.SignedAngle(end - centre, Vector.ZAxis);
                bool flip = false;

                if ((start - centre).CrossProduct(arc.StartDir()).Z < 0)
                {
                    double temp = startAngle;
                    startAngle = endAngle;
                    endAngle = temp;
                    Point tempPt = start;
                    start = end;
                    end = tempPt;

                    flip = true;
                }
                double pi2 = Math.PI * 2;
                if (start.X > x)
                {
                    Arc toSplit = (Arc)results[1];
                    results.RemoveAt(1);
                    if (end.X > x)
                    {
                        // Both to the right
                        results.Insert(0, Geometry.Create.Arc(toSplit.CoordinateSystem, arc.Radius, startAngle, toSplit.EndAngle < startAngle ? toSplit.EndAngle + pi2 : toSplit.EndAngle));
                        results.Add(Geometry.Create.Arc(toSplit.CoordinateSystem, arc.Radius, toSplit.StartAngle, endAngle < toSplit.StartAngle ? endAngle + pi2 : endAngle));
                    } else
                    {
                        results.RemoveAt(0);
                        // start right, end left
                        results.Add(Geometry.Create.Arc(toSplit.CoordinateSystem, arc.Radius, startAngle, toSplit.EndAngle < startAngle ? toSplit.EndAngle + pi2 : toSplit.EndAngle));
                        results.Add(Geometry.Create.Arc(toSplit.CoordinateSystem, arc.Radius, toSplit.EndAngle, endAngle < toSplit.EndAngle ? endAngle + pi2 : endAngle));
                    }
                } else
                {
                    Arc toSplit = (Arc)results[0];
                    results.RemoveAt(0);
                    if (end.X > x)
                    {
                        results.RemoveAt(0);
                        // start left, end right 
                        results.Add(Geometry.Create.Arc(toSplit.CoordinateSystem, arc.Radius, startAngle + pi2 < toSplit.EndAngle ? startAngle + pi2 : startAngle, toSplit.EndAngle));
                        results.Add(Geometry.Create.Arc(toSplit.CoordinateSystem, arc.Radius, toSplit.EndAngle, endAngle < toSplit.EndAngle ? endAngle + pi2 : endAngle));
                    }
                    else
                    {
                        // both left
                        results.Insert(0, Geometry.Create.Arc(toSplit.CoordinateSystem, arc.Radius, startAngle + pi2 < toSplit.EndAngle ? startAngle + pi2 : startAngle, toSplit.EndAngle));
                        results.Add(Geometry.Create.Arc(toSplit.CoordinateSystem, arc.Radius, toSplit.StartAngle, endAngle < toSplit.StartAngle ? endAngle + pi2 : endAngle));
                    }
                }

                // Flip test
                if (flip)
                    results = results.Select(y => y.IFlip()).Reverse().ToList();
            }
            else
                results.Add(arc.Clone());

                return results;
        }

        /***************************************************/

        private static IEnumerable<ICurve> SplitAtX(Circle circle, double x, double tol = Tolerance.Distance)
        {
            // Assumes 2D
            List<ICurve> results = new List<ICurve>();

            if (circle.Centre.X + circle.Radius > x &&
                circle.Centre.X - circle.Radius < x)
            {
                Cartesian mid = new Cartesian() { Origin = circle.Centre };
                double startAngle = Math.Asin((circle.Centre.X - x) / circle.Radius) + Math.PI * 0.5;
                double endAngle = -startAngle;
                results.Add(Geometry.Create.Arc(mid, circle.Radius, startAngle, endAngle + 2 * Math.PI)); //Left
                results.Add(Geometry.Create.Arc(mid, circle.Radius, endAngle, startAngle)); // Rigth

                if (circle.Normal.Z < 0)
                    results = results.Select(y => y.IFlip()).ToList();
            }
            else
                results.Add(circle.Clone());

            return results;
        }

        /***************************************************/

        private static IEnumerable<ICurve> SplitAtX(Line line, double x, double tol = Tolerance.Distance)
        {
            // Assumes 2D
            List<ICurve> results = new List<ICurve>();

            if (line.Start.X < x && line.End.X > x ||
                line.Start.X > x && line.End.X < x)
            {
                Point ptOn = Geometry.Query.PointAtX(line.Start, line.End, x);
                results.Add(new Line() { Start = line.Start, End = ptOn });
                results.Add(new Line() { Start = ptOn, End = line.End });
            }
            else
                results.Add(line.Clone());

            return results;
        }

        /***************************************************/
        
        private static IEnumerable<ICurve> SplitAtX(ICurve curve, double x, double tol = Tolerance.Distance)
        {
            throw new NotImplementedException("Plastic modulus could not be calculated. Can not perform necessary splitting operation on curves of type " + curve.GetType().Name + ".");
        }

        /***************************************************/

        private static bool IntersectsArc(this Arc arc, double xValue)
        {

            if (!arc.IsValid())
                throw new Exception("Invalid Arc");

            Circle circle = new Circle { Centre = arc.CoordinateSystem.Origin, Normal = arc.CoordinateSystem.Z, Radius = arc.Radius };

            if (circle.Centre.X + circle.Radius < xValue &&
                circle.Centre.X - circle.Radius > xValue)
            {
                return false;
            }

            Point start = arc.StartPoint();
            Point end = arc.EndPoint();

            double xMax, xMin;

            //Get m in and max values from start and end points
            if (start.X > end.X)
            {
                xMax = start.X;
                xMin = end.X;
            }
            else
            {
                xMax = end.X;
                xMin = start.X;
            }

            //Circular arc parameterised to
            //A(theta) = C+r*cos(theta)*xloc+r*sin(theta)*yloc
            //where: C = centre point
            //r - radius
            //xloc - local x-axis unit vector
            //yloc - local y-axis unit vector
            //A - point on the circle

            Vector x = start - circle.Centre;
            Vector y = circle.Normal.CrossProduct(x);

            Vector endV = end - circle.Centre;

            double angle = x.SignedAngle(endV, circle.Normal);

            angle = angle < 0 ? angle + Math.PI * 2 : angle;

            double a1, b1;

            a1 = x.X;
            b1 = y.X;


            //Finding potential extreme values for x, y and z. Solving for A' = 0

            //Extreme x
            double theta = Math.Abs(a1) > Tolerance.Angle ? Math.Atan(b1 / a1) : Math.PI / 2;
            while (theta < angle)
            {
                if (theta > 0)
                {
                    double xTemp = circle.Centre.X + Math.Cos(theta) * a1 + Math.Sin(theta) * b1;
                    xMax = Math.Max(xMax, xTemp);
                    xMin = Math.Min(xMin, xTemp);
                }
                theta += Math.PI;
            }

            return (xMax > xValue && xMin < xValue);
        }

        /***************************************************/


    }
}
