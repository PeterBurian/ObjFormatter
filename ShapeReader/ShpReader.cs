using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace ShapeReader
{
    /// <summary>
    /// https://github.com/NetTopologySuite/NetTopologySuite.IO.ShapeFile
    /// </summary>
    public class ShpReader
    {
        private string source;

        private Vector3 center;

        private Dictionary<int, Vector3> points;

        private readonly Dictionary<int, Vector3> normals;

        private List<List<Tuple<int, int, int>>> faces;
        
        Vector3 Center => center;

        List<Vector3> Points => points.Values.ToList();

        List<Vector3> Normals => normals.Values.ToList();

        List<List<Tuple<int, int, int>>> Faces => faces;


        public ShpReader(string source)
        {
            this.source = source;

            points = new Dictionary<int, Vector3>();
            normals = new Dictionary<int, Vector3>();
            faces = new List<List<Tuple<int, int, int>>>();
        }

        public bool ReadShp()
        {
            if (!String.IsNullOrEmpty(source))
            {
                if (File.Exists(source))
                {
                    GeometryCollection geometry = ReadShape();

                    if (geometry != null && !geometry.IsEmpty)
                    {
                        //Set vectors
                        ReadCoordinates(geometry.Coordinates);

                        //Set center
                        Point centroid = geometry.Centroid;
                        center = new Vector3((float)centroid.X, (float)centroid.Y, (float)centroid.Z);

                        //Set faces
                        ReadFaces(geometry.Geometries);
                    }
                    else
                    {
                        Console.WriteLine("Shp geometry is null or empty!");
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine("Source file doesn't exists!");
                }
            }
            else
            {
                Console.WriteLine("Source can not be null!");
            }
            return false;
        }

        private void ReadCoordinates(Coordinate[] coordinates)
        {
            if (coordinates != null)
            {
                for (int i = 0; i < coordinates.Length; i++)
                {
                    Vector3 vector = GetVector(coordinates[i]);
                    points.Add(i, vector);
                }
            }
            else
            {
                Console.WriteLine("Coordinates are null or empty!");
            }
        }

        private void ReadFaces(Geometry[] geometries)
        {
            if (geometries != null)
            {
                for (int i = 0; i < geometries.Length; i++)
                {
                    List<Tuple<int, int, int>> faceLst = new List<Tuple<int, int, int>>();

                    Geometry geom = geometries[i];

                    //Read the polygons only 
                    if (geom.GeometryType == "Polygon")
                    {
                        Triangulator2D triangulator = new Triangulator2D(geom.Coordinates);
                        List<List<Coordinate>> coordinates = triangulator.TriangulateFace();

                        //Add normals
                        int nIndex = 0;
                        foreach (List<Coordinate> triangle in coordinates)
                        {
                            for (int j = 0; j < 3; i++)
                            {
                                int nextIdx = (j + 1) % 3;
                                int prevIdx = (j + 3 - 1) % 3;

                                Vector3 current = GetVector(triangle[j]);
                                Vector3 prev = GetVector(triangle[prevIdx]);
                                Vector3 next = GetVector(triangle[nextIdx]);

                                Vector3 cross = Vector3.Cross(next - current, prev - current);
                                cross = Vector3.Normalize(cross);

                                normals.Add(nIndex++, cross);
                            }
                        }

                        for (int j = 0; j < geom.Coordinates.Length; j++)
                        {
                            Coordinate coordinate = geom.Coordinates[j];
                            int vIndex = GetIndexOfVertex(GetVector(coordinate));

                            var facePart = new Tuple<int, int, int>(vIndex, -1, -1);
                            faceLst.Add(facePart);
                        }

                        
                    }
                    faces.Add(faceLst);
                }
            }
            else
            {
                Console.WriteLine("Geometries are null!");
            }
        }

        private int GetIndexOfVertex(Vector3 vec)
        {
            foreach (KeyValuePair<int, Vector3> item in points)
            {
                //I hope there is no inconsistence in shp and the coordinates in the list and in the geoms are the same
                if (item.Value.X == vec.X && item.Value.Y == vec.Y && item.Value.Z == vec.Z)
                {
                    return item.Key;
                }
            }
            return -1;
        }

        private Vector3 GetVector(Coordinate coordinate)
        { 
            return new Vector3((float)coordinate.X, (float)coordinate.Y, (float)coordinate.Z);
        }

        private GeometryCollection ReadShape()
        {
            var reader = new ShapefileReader(source);
            var geometries = reader.ReadAll();
            return geometries;
        }
    }

    
}
