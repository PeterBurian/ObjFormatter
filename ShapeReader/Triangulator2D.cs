using DelaunatorSharp;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace ShapeReader
{
    public class Triangulator2D
    {
        PointAdapter[] face;

        public Triangulator2D(Coordinate[] coordinates)
        {
            face = new PointAdapter[coordinates.Length];

            for (int i = 0; i < coordinates.Length; i++)
            {
                face[i] = coordinates[i];
            }
        }

        public void TriangulateFace() 
        {
            Delaunator delaunator = new Delaunator(face);
            List<ITriangle> triangles = delaunator.GetTriangles().ToList();
        }

        public class PointAdapter : IPoint
        {
            private double x;
            private double y;

            public double X { get => x; set => x = value; }
            public double Y { get => y; set => y = value; }

            public PointAdapter(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public PointAdapter(Coordinate coordinate)
            {
                x = coordinate.X;
                y = coordinate.Y;
            }

            public static implicit operator PointAdapter(Coordinate c) => new PointAdapter(c);
        }
    }
}
