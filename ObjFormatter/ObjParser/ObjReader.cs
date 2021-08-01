using ObjFormatter.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace ObjFormatter.ObjParser
{
    public class ObjReader : IReader
    {
        private readonly string source;
        private readonly List<Vector3> points;
        private readonly List<Vector3> normals;
        
        // vertex index - vertex texture index - normal index
        private readonly List<List<Tuple<int, int, int>>> faces;

        private Vector3 min;
        private Vector3 max;

        private delegate void VecMthd(Vector3 vector);

        public Vector3 Center => (min + max) / 2.0f;

        public List<Vector3> Points => points;

        public List<Vector3> Normals => normals;

        public List<List<Tuple<int, int, int>>> Faces => faces;

        public string Source => source;


        public ObjReader(string source)
        {
            this.source = source;
            points = new List<Vector3>();
            normals = new List<Vector3>();
            faces = new List<List<Tuple<int, int, int>>>();

            min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        }

        public void Parse()
        {
            int CountSlash(string str)
            {
                int count = 0;

                if (!string.IsNullOrEmpty(str))
                {
                    for (int i = 0; i < str.Length; i++)
                    {
                        if (str[i] == '/')
                        {
                            count++;
                        }
                    }
                }
                return count;
            }

            if (!string.IsNullOrEmpty(source))
            {
                if (File.Exists(source))
                {
                    using (StreamReader reader = new StreamReader(source))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            String[] data = line.Split(' ');

                            //Read vertices
                            if (line.StartsWith("v "))
                            {
                                GetVector(data, (pt) =>
                                {
                                    points.Add(pt);

                                    //Get min value
                                    if (pt.X < min.X) min.X = pt.X;
                                    if (pt.Y < min.Y) min.Y = pt.Y;
                                    if (pt.Z < min.Z) min.Z = pt.Z;

                                    //Get max value
                                    if (pt.X > max.X) max.X = pt.X;
                                    if (pt.Y > max.Y) max.Y = pt.Y;
                                    if (pt.Z > max.Z) max.Z = pt.Z;

                                });
                            }
                            else if (line.StartsWith("vn ")) //Read normals
                            {
                                GetVector(data, (pt) =>
                                {
                                    normals.Add(pt);
                                });
                            }
                            else if (line.StartsWith("f ")) //Read faces
                            {
                                if (data[1].Contains("//"))
                                {
                                    //It contains the vertex and the normal vector indicies starts from 1
                                    //f v1//vn1 v2//vn2 v3//vn3

                                    // !!! At this moment Environsense uses this case

                                    List<Tuple<int, int, int>> faceLst = new List<Tuple<int, int, int>>();

                                    String[] items = line.Split(' ');

                                    for (int i = 1; i < items.Length; i++)
                                    {
                                        String[] faceData = items[i].Split(new string[1] { @"//" }, StringSplitOptions.None);

                                        if (faceData != null 
                                        && faceData.Length == 2 
                                        && Int32.TryParse(faceData[0], out int vIndex) 
                                        && Int32.TryParse(faceData[1], out int normalIdx))
                                        {
                                            var facePart = new Tuple<int, int, int>(vIndex, -1, normalIdx);
                                            faceLst.Add(facePart);
                                        }
                                    }
                                    faces.Add(faceLst);
                                }
                                else if (data[1].Contains("/"))
                                {
                                    int cnt = CountSlash(data[1]);

                                    if (cnt == 1)
                                    {
                                        //It contains the vertex and the vertex texture indicies starts from 1
                                        //f v1/vt1 v2/vt2 v3/vt3 ...

                                        throw new NotImplementedException("f v1/vt1 v2/vt2 v3/vt3 NOT IMPLEMENTED");
                                    }
                                    else if (cnt == 2)
                                    {
                                        //It contains the vertex, the vertex texture and normal of vertex indicies starts from 1
                                        //f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3

                                        throw new NotImplementedException("f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3 NOT IMPLEMENTED");
                                    }
                                }
                                else
                                {
                                    //It contains the vertex index starts from 1
                                    //f v1 v2 v3
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Source file doesn't exist!");
                }
            }
            else
            {
                Console.WriteLine("Source path is null or empty!");
            }
        }

        private void GetVector(String[] data, VecMthd mthd)
        {
            try
            {
                if (Single.TryParse(data[1], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float x)
                &&  Single.TryParse(data[2], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float y)
                &&  Single.TryParse(data[3], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float z))
                {
                    Vector3 pt = new Vector3(x, y, z);
                    mthd?.Invoke(pt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
