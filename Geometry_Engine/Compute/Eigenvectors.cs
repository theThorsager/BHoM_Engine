﻿using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        public static Vector[] Eigenvectors(this TransformMatrix matrix, double tolerance = Tolerance.Distance)
        {
            return matrix.Matrix.Eigenvectors(tolerance);
        }


        /***************************************************/
        /****   Private Methods                         ****/
        /***************************************************/

        private static Vector[] Eigenvectors(this double[,] matrix, double tolerance = Tolerance.Distance)
        {
            double[] eigenvalues = matrix.Eigenvalues(tolerance);
            if (eigenvalues == null)
                return null;

            double a = matrix[0, 0];
            double b = matrix[0, 1];
            double c = matrix[0, 2];
            double d = matrix[1, 1];
            double e = matrix[1, 2];
            double f = matrix[2, 2];

            double sqTol = tolerance * tolerance;
            Vector[] result = new Vector[6];
            for (int i = 0; i < 3; i++)
            {
                double k = eigenvalues[i];
                double[,] equations = {{ a - k, b, c },
                                       { b, d - k, e },
                                       { c, e, f - k }};

                double REFTolerance = equations.REFTolerance(tolerance);
                double[,] REF = equations.RowEchelonForm(true, REFTolerance);
                
                double y = -REF[1, 2];
                double x = -REF[0, 2] - y * REF[0, 1];

                result[2 * i] = x * x + y * y <= sqTol ? new Vector { X = 0, Y = 0, Z = 1 } : new Vector { X = x, Y = y, Z = 0 }.Normalise();
                result[2 * i + 1] = new Vector { X = x, Y = y, Z = 1 }.Normalise();
            }
            return result;
        }

        /***************************************************/
    }
}
