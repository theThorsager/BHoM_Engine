﻿using BH.oM.Geometry;
using System.Linq;
using System.Collections.Generic;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsClockwise(this Polyline polyline, Vector normal, double tolerance = Tolerance.Distance)
        {
            if (!polyline.IsClosed(tolerance)) throw new Exception("The polyline is not closed. IsClockwise method is relevant only to closed curves.");
            List<Point> cc = polyline.DiscontinuityPoints(tolerance);
            Vector dir1 = (cc[0] - cc.Last()).Normalise();
            Vector dir2;
            double angleTot = 0;
            for (int i = 1; i < cc.Count; i++)
            {
                dir2 = (cc[i] - cc[i - 1]).Normalise();
                angleTot += dir1.SignedAngle(dir2, normal);
                dir1 = dir2.Clone();
            }
            return angleTot > 0;
        }

        /***************************************************/

        public static bool IsClockwise(this PolyCurve curve, Vector normal, double tolerance = Tolerance.Distance)
        {
            if (!curve.IsClosed(tolerance)) throw new Exception("The curve is not closed. IsClockwise method is relevant only to closed curves.");
            List<Point> cc = curve.ControlPoints().CullDuplicates(tolerance);        //TODO: PolyCurve.DiscontinuityPoints() would be more robust
            Vector dir1 = (cc[0] - cc.Last()).Normalise();
            Vector dir2;
            double angleTot = 0;
            for (int i = 1; i < cc.Count; i++)
            {
                dir2 = (cc[i] - cc[i - 1]).Normalise();
                angleTot += dir1.SignedAngle(dir2, normal);
                dir1 = dir2.Clone();
            }
            return angleTot > 0;
        }

        /***************************************************/

        public static bool IsClockwise(this Polyline polyline, Point viewPoint, double tolerance = Tolerance.Distance)
        {
            Plane plane = polyline.FitPlane(tolerance);

            Point projectedPoint = viewPoint.Project(plane);
            Vector vector = (projectedPoint - viewPoint).Normalise();

            return IsClockwise(polyline, vector);
        }

        /***************************************************/

        public static bool IsClockwise(this PolyCurve curve, Point viewPoint, double tolerance = Tolerance.Distance)
        {
            Plane plane = curve.FitPlane(tolerance);

            Point projectedPoint = viewPoint.Project(plane);
            Vector vector = (projectedPoint - viewPoint).Normalise();

            return IsClockwise(curve, vector);
        }

         /***************************************************/

        public static bool IsClockwise(this Arc arc, Vector axis, double tolerance = Tolerance.Distance)
        {
            Point centre = arc.Centre(tolerance);
            Vector a = arc.Start - centre;
            Vector b = arc.End - centre;
            Vector crossproduct = a.CrossProduct(b);

            return ((crossproduct.DotProduct(axis) < 0) != (arc.Angle() > Math.PI));       
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/
        
        public static bool IIsClockwise(this ICurve curve, Vector axis, double tolerance = Tolerance.Distance)
        {
            return IsClockwise(curve as dynamic, axis, tolerance);
        }

        /***************************************************/
    }
}
