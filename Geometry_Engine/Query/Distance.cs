﻿using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Distance(this Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /***************************************************/

        public static double SquareDistance(this Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        /***************************************************/

        public static double Distance(this Vector a, Vector b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /***************************************************/

        public static double SquareDistance(this Vector a, Vector b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        /***************************************************/

        public static double Distance(this Point a, Plane plane)
        {
            Vector normal = plane.Normal.Normalise();
            return normal.DotProduct(a - plane.Origin);
        }

        /***************************************************/

        public static double Distance(this Point a, Line line)
        {
            return a.Distance(line.IClosestPoint(a));
        }

        /***************************************************/

        public static double SquareDistance(this Point a, Line line)
        {
            return a.SquareDistance(line.IClosestPoint(a));
        }

        /***************************************************/

        public static double Distance(this Line line, Line other)
        {
            Point intersection = line.LineIntersection(other, false);
            if (intersection != null)
            {
                return 0;
            }
            else
            {
                List<double> distances = new List<double>();        //TODO: Can we do better than this?
                distances.Add(line.Start.Distance(other));
                distances.Add(line.End.Distance(other));
                distances.Add(other.Start.Distance(line));
                distances.Add(other.End.Distance(line));

                return distances.Min();
            }
        }
    }
}
