using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Dimension
{
    /// <summary>
    /// Contains all the procedures related to 3D shapes.
    /// </summary>
    class Shapes
    {

        #region Cylinder

        /// <summary>
        /// Creates a smooth cylinder.
        /// </summary>
        /// 
        /// <param name="mesh">
        /// Shape to be modelled.
        /// </param>
        /// 
        /// <param name="end_point">
        /// Ending point.
        /// </param>
        /// 
        /// <param name="axis">
        /// Direction of the cylinder in 3D space
        /// </param>
        /// 
        /// <param name="radius">
        /// Radius of the cylinder.
        /// </param>
        /// 
        /// <param name="num_sides">
        /// Simply specifies how smooth should the cylinder be.
        /// </param>
        /// 
        /// <returns>
        /// Smooth cylinder.
        /// </returns>
        public MeshGeometry3D AddSmoothCylinder(MeshGeometry3D mesh, Point3D end_point, Vector3D axis, double radius, int num_sides)
        {
            // Get two vectors perpendicular to the axis.
            Vector3D v1;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);

            Vector3D v2 = Vector3D.CrossProduct(v1, axis);

            // Make the vectors have length radius.
            v1 *= (radius / v1.Length);
            v2 *= (radius / v2.Length);

            // Make the top end cap.
            // Make the end point.
            int pt0 = mesh.Positions.Count; // Index of end_point.
            mesh.Positions.Add(end_point);

            // Make the top points.
            double theta = 0;
            double dtheta = 2 * Math.PI / num_sides;

            for (int i = 0; i < num_sides; i++)
            {
                mesh.Positions.Add(end_point + Math.Cos(theta) * v1 + Math.Sin(theta) * v2);
                theta += dtheta;
            }

            // Make the top triangles.
            int pt1 = mesh.Positions.Count - 1; // Index of last point.
            int pt2 = pt0 + 1;                  // Index of first point.

            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(pt0);
                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt2);
                pt1 = pt2++;
            }

            // Make the bottom end cap.
            // Make the end point.
            pt0 = mesh.Positions.Count; // Index of end_point2.
            Point3D end_point2 = end_point + axis;
            mesh.Positions.Add(end_point2);

            // Make the bottom points.
            theta = 0;

            for (int i = 0; i < num_sides; i++)
            {
                mesh.Positions.Add(end_point2 + Math.Cos(theta) * v1 + Math.Sin(theta) * v2);
                theta += dtheta;
            }

            // Make the bottom triangles.
            theta = 0;
            pt1 = mesh.Positions.Count - 1; // Index of last point.
            pt2 = pt0 + 1;                  // Index of first point.

            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(num_sides + 1);    // end_point2
                mesh.TriangleIndices.Add(pt2);
                mesh.TriangleIndices.Add(pt1);
                pt1 = pt2++;
            }

            // Make the sides.
            // Add the points to the mesh.
            int first_side_point = mesh.Positions.Count;
            theta = 0;

            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                mesh.Positions.Add(p1);
                Point3D p2 = p1 + axis;
                mesh.Positions.Add(p2);
                theta += dtheta;
            }

            // Make the side triangles.
            pt1 = mesh.Positions.Count - 2;
            pt2 = pt1 + 1;
            int pt3 = first_side_point;
            int pt4 = pt3 + 1;

            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt2);
                mesh.TriangleIndices.Add(pt4);

                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt4);
                mesh.TriangleIndices.Add(pt3);

                pt1 = pt3;
                pt3 += 2;
                pt2 = pt4;
                pt4 += 2;
            }

            return mesh;
        }

        #endregion

        #region Sphere

        /// <summary>
        /// Creates a sphere.
        /// </summary>
        /// 
        /// <param name="mesh">
        /// Shape to be modelled.
        /// </param>
        /// 
        /// <param name="center">
        /// Center to the sphere.
        /// </param>
        /// 
        /// <param name="radius">
        /// Radius of the sphere.
        /// </param>
        /// 
        /// <param name="num_phi">
        /// Determines how smooth the sphere is.
        /// </param>
        /// 
        /// <param name="num_theta">
        /// Determines how smooth the sphere is.
        /// </param>
        /// 
        /// <returns>
        /// Smooth sphere.
        /// </returns>
        public MeshGeometry3D AddSphere(MeshGeometry3D mesh, Point3D center, double radius, int num_phi, int num_theta)
        {
            double phi0, theta0;
            double dphi = Math.PI / num_phi;
            double dtheta = 2 * Math.PI / num_theta;

            phi0 = 0;
            double y0 = radius * Math.Cos(phi0);
            double r0 = radius * Math.Sin(phi0);
            for (int i = 0; i < num_phi; i++)
            {
                double phi1 = phi0 + dphi;
                double y1 = radius * Math.Cos(phi1);
                double r1 = radius * Math.Sin(phi1);

                // Point ptAB has phi value A and theta value B.
                // For example, pt01 has phi = phi0 and theta = theta1.
                // Find the points with theta = theta0.
                theta0 = 0;
                Point3D pt00 = new Point3D(
                    center.X + r0 * Math.Cos(theta0),
                    center.Y + y0,
                    center.Z + r0 * Math.Sin(theta0));
                Point3D pt10 = new Point3D(
                    center.X + r1 * Math.Cos(theta0),
                    center.Y + y1,
                    center.Z + r1 * Math.Sin(theta0));
                for (int j = 0; j < num_theta; j++)
                {
                    // Find the points with theta = theta1.
                    double theta1 = theta0 + dtheta;
                    Point3D pt01 = new Point3D(
                        center.X + r0 * Math.Cos(theta1),
                        center.Y + y0,
                        center.Z + r0 * Math.Sin(theta1));
                    Point3D pt11 = new Point3D(
                        center.X + r1 * Math.Cos(theta1),
                        center.Y + y1,
                        center.Z + r1 * Math.Sin(theta1));

                    // Create the triangles.
                    AddSmoothTriangle(mesh, new Dictionary<Point3D, int>(), pt00, pt11, pt10);
                    AddSmoothTriangle(mesh, new Dictionary<Point3D, int>(), pt00, pt01, pt11);

                    // Move to the next value of theta.
                    theta0 = theta1;
                    pt00 = pt01;
                    pt10 = pt11;
                }

                // Move to the next value of phi.
                phi0 = phi1;
                y0 = y1;
                r0 = r1;
            }

            return mesh;

        }

        // Add a triangle to the indicated mesh.
        // Reuse points so triangles share normals.
        private void AddSmoothTriangle(MeshGeometry3D mesh, Dictionary<Point3D, int> dict, Point3D point1, Point3D point2, Point3D point3)
        {
            int index1, index2, index3;

            // Find or create the points.
            if (dict.ContainsKey(point1)) index1 = dict[point1];
            else
            {
                index1 = mesh.Positions.Count;
                mesh.Positions.Add(point1);
                dict.Add(point1, index1);
            }

            if (dict.ContainsKey(point2)) index2 = dict[point2];
            else
            {
                index2 = mesh.Positions.Count;
                mesh.Positions.Add(point2);
                dict.Add(point2, index2);
            }

            if (dict.ContainsKey(point3)) index3 = dict[point3];
            else
            {
                index3 = mesh.Positions.Count;
                mesh.Positions.Add(point3);
                dict.Add(point3, index3);
            }

            // If two or more of the points are
            // the same, it's not a triangle.
            if ((index1 == index2) ||
                (index2 == index3) ||
                (index3 == index1)) return;

            // Create the triangle.
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index2);
            mesh.TriangleIndices.Add(index3);
        }

        #endregion

        #region Cube

        /// <summary>
        /// Creates a cube.
        /// </summary>
        /// 
        /// <param name="center">
        /// Position at which the center of the cube will be.
        /// </param>
        /// 
        /// <param name="sideLength">
        /// length, width and height of cube.
        /// </param>
        /// 
        /// <returns>
        /// Cube based on parameters.
        /// </returns>
        public MeshGeometry3D AddCube(Point3D center, float sideLength)
        {
            // cube to be modelled.
            var cube = new MeshGeometry3D();

            #region Positions

            // stores half of the sidelength.
            var halfLength = sideLength / 2;

            // Stores the first vertex of the shapes nased on center.
            var Vertex = new Point3D(center.X - halfLength,
                                     center.Y + halfLength,
                                     center.Z + halfLength);

            // Adds first vertex.
            cube.Positions.Add(Vertex);

            // Vertex below the previous one is stored.
            Vertex.Y -= sideLength;

            // Adds vertex.
            cube.Positions.Add(Vertex);

            // Vertex to the right of the previous one is stored.
            Vertex.X += sideLength;

            // Adds vertex.
            cube.Positions.Add(Vertex);

            // Vertex above the previous one is stored.
            Vertex.Y += sideLength;

            // Adds vertex.
            cube.Positions.Add(Vertex);

            // Vertex behind the previous one is stored.
            Vertex.Z -= sideLength;

            // Adds vertex.
            cube.Positions.Add(Vertex);

            // Vertex below the previous one is stored.
            Vertex.Y -= sideLength;

            // Adds vertex.
            cube.Positions.Add(Vertex);

            // Vertex to the left of the previous one is stored.
            Vertex.X -= sideLength;

            // Adds vertex.
            cube.Positions.Add(Vertex);

            // Vertex above the previous one is stored.
            Vertex.Y += sideLength;

            // Adds vertex.
            cube.Positions.Add(Vertex);

            #endregion

            #region TrinagleIndices

            /* All of these lines of codes store the indices in anti-clockwise order
               to form 'triangles' */

            cube.TriangleIndices.Add(0);
            cube.TriangleIndices.Add(1);
            cube.TriangleIndices.Add(2);
            cube.TriangleIndices.Add(0);
            cube.TriangleIndices.Add(2);
            cube.TriangleIndices.Add(3);
            cube.TriangleIndices.Add(3);
            cube.TriangleIndices.Add(2);
            cube.TriangleIndices.Add(5);
            cube.TriangleIndices.Add(3);
            cube.TriangleIndices.Add(5);
            cube.TriangleIndices.Add(4);
            cube.TriangleIndices.Add(4);
            cube.TriangleIndices.Add(5);
            cube.TriangleIndices.Add(6);
            cube.TriangleIndices.Add(4);
            cube.TriangleIndices.Add(6);
            cube.TriangleIndices.Add(7);
            cube.TriangleIndices.Add(7);
            cube.TriangleIndices.Add(6);
            cube.TriangleIndices.Add(1);
            cube.TriangleIndices.Add(7);
            cube.TriangleIndices.Add(1);
            cube.TriangleIndices.Add(0);
            cube.TriangleIndices.Add(0);
            cube.TriangleIndices.Add(3);
            cube.TriangleIndices.Add(4);
            cube.TriangleIndices.Add(0);
            cube.TriangleIndices.Add(4);
            cube.TriangleIndices.Add(7);
            cube.TriangleIndices.Add(1);
            cube.TriangleIndices.Add(6);
            cube.TriangleIndices.Add(5);
            cube.TriangleIndices.Add(1);
            cube.TriangleIndices.Add(5);
            cube.TriangleIndices.Add(2);

            #endregion

            // returns the modelled cube.
            return cube;
        }

        #endregion

        #region Pyramid

        /// <summary>
        /// Creates a pyramid.
        /// </summary>
        /// 
        /// <param name="center">
        /// Position at which the center of the pyramid will be.
        /// </param>
        /// 
        /// <param name="sideLength">
        /// length, width and height of pyramid.
        /// </param>
        /// 
        /// <returns>
        /// Modelled pyramid based on parameters.
        /// </returns>
        public MeshGeometry3D AddPyramid(Point3D center, float sideLength)
        {

            // pyramid to be modelled.
            var pyramid = new MeshGeometry3D();

            #region Positions

            // stores half of the sidelength.
            var halfside = sideLength / 2;

            // Stores the first vertex of the shapes nased on center.
            var Vertex = new Point3D(center.X, center.Y + halfside, center.Z);

            // Adds first vertex.
            pyramid.Positions.Add(Vertex);

            // Stores bottom left up-front vertex.
            Vertex.Y -= sideLength;
            Vertex.X -= halfside;
            Vertex.Z += halfside;

            // Adds the vertex.
            pyramid.Positions.Add(Vertex);

            // Stores vertex right to the previous one.
            Vertex.X += sideLength;

            // Adds vertex.
            pyramid.Positions.Add(Vertex);

            // Stores vertex behind the previous one.
            Vertex.Z -= sideLength;

            // Adds vertex.
            pyramid.Positions.Add(Vertex);

            // Stores vertex left to the previous one.
            Vertex.X -= sideLength;

            // Adds vertex.
            pyramid.Positions.Add(Vertex);

            #endregion

            #region Triangle Indices

            /* All of these lines of codes store the indices in anti-clockwise order
               to form 'triangles' */

            pyramid.TriangleIndices.Add(0);
            pyramid.TriangleIndices.Add(1);
            pyramid.TriangleIndices.Add(2);

            pyramid.TriangleIndices.Add(2);
            pyramid.TriangleIndices.Add(3);
            pyramid.TriangleIndices.Add(0);

            pyramid.TriangleIndices.Add(0);
            pyramid.TriangleIndices.Add(3);
            pyramid.TriangleIndices.Add(4);

            pyramid.TriangleIndices.Add(1);
            pyramid.TriangleIndices.Add(0);
            pyramid.TriangleIndices.Add(4);

            pyramid.TriangleIndices.Add(3);
            pyramid.TriangleIndices.Add(2);
            pyramid.TriangleIndices.Add(1);

            pyramid.TriangleIndices.Add(1);
            pyramid.TriangleIndices.Add(4);
            pyramid.TriangleIndices.Add(3);

            #endregion

            // returns modelled pyramid
            return pyramid;
        }

        #endregion
    }
}



