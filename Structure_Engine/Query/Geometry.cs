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

using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties.Section;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point Geometry(this Node node)
        {
            return node.Position;
        }

        /***************************************************/

        public static Line Geometry(this Bar bar)
        {
            return bar.Centreline();
        }

        /***************************************************/

        public static ICurve Geometry(this Edge edge)
        {
            return edge.Curve;
        }

        /***************************************************/

        public static IGeometry Geometry(this PanelFreeForm contour)
        {
            return contour.Surface;
        }

        /***************************************************/

        public static CompositeGeometry Geometry(this ConcreteSection section)
        {
            if (section.SectionProfile.Edges.Count == 0)
                return null;

            CompositeGeometry geom = Engine.Geometry.Create.CompositeGeometry(section.SectionProfile.Edges);
            if(section.Reinforcement != null)
                geom.Elements.AddRange(section.Layout().Elements);

            return geom;
        }

        /***************************************************/

        public static IGeometry Geometry(this SteelSection section)
        {
            return new CompositeGeometry { Elements = section.SectionProfile.Edges.ToList<IGeometry>() };
        }

        /***************************************************/

        public static BH.oM.Geometry.Mesh Geometry(this MeshFace meshFace)
        {

            return new BH.oM.Geometry.Mesh()
            {
                Vertices = meshFace.Nodes.Select(x => x.Position).ToList(),
                Faces = meshFace.Nodes.Count == 3 ? new List<Face>() { new Face() { A = 0, B = 1, C = 2 } } : new List<Face>() { new Face() { A = 0, B = 1, C = 2, D = 3 } }
            };
                
        }

        /***************************************************/

        public static IGeometry Geometry(this RigidLink link)
        {
            List<IGeometry> lines = new List<IGeometry>();

            foreach (Node sn in link.SlaveNodes)
            {
                lines.Add(new Line() { Start = link.MasterNode.Position, End = sn.Position });
            }
            return new CompositeGeometry() { Elements = lines };
        }

        /***************************************************/

        public static ICurve Geometry(this FramingElement element)
        {
            return element.LocationCurve;
        }

        /***************************************************/

        public static Mesh Geometry(this FEMesh feMesh)
        {
            Mesh mesh = new Mesh();

            mesh.Vertices = feMesh.Nodes.Select(x => x.Position).ToList();

            foreach (FEMeshFace feFace in feMesh.MeshFaces)
            {
                if (feFace.NodeListIndices.Count < 3)
                {
                    Reflection.Compute.RecordError("Insuffiecient node indices");
                    continue;
                }
                if (feFace.NodeListIndices.Count > 4)
                {
                    Reflection.Compute.RecordError("To high number of node indices. Can only handle triangular and quads");
                    continue;
                }

                Face face = new Face();

                face.A = feFace.NodeListIndices[0];
                face.B = feFace.NodeListIndices[1];
                face.C = feFace.NodeListIndices[2];

                if (feFace.NodeListIndices.Count == 4)
                    face.D = feFace.NodeListIndices[3];

                mesh.Faces.Add(face);
            }

            return mesh;
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        public static IGeometry Geometry(this ISectionProperty section)
        {
            return Geometry(section as dynamic);
        }

        /***************************************************/
    }

}
