using ObjFormatter.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;

namespace ObjFormatter.ObjParser
{
    public class ObjWriter : IWriter
    {
        private readonly string source;

        private Vector3 center;
        private List<Vector3> points;
        private List<Vector3> normals;
        private List<List<Tuple<int, int, int>>> faces;
        private List<Vector3> diffs;

        public ObjWriter(string source)
        {
            this.source = source;

            this.center = Vector3.Zero;
            this.points = new List<Vector3>();
            this.normals = new List<Vector3>();
            this.faces = new List<List<Tuple<int, int, int>>>();
            this.diffs = new List<Vector3>();
        }

        public void SetPoints(List<Vector3> points)
        {
            this.points = points;
        }

        public void SetNormals(List<Vector3> normals)
        {
            this.normals = normals;
        }

        public void SetFaces(List<List<Tuple<int, int, int>>> faces)
        {
            this.faces = faces;
        }

        public void SetCenter(Vector3 center)
        {
            this.center = center;
        }

        public void Write(string target)
        {
            Write(target, true, false, true, false);
        }

        public void Write(string target, bool useRelativeCoordinates, bool rotateYZ, bool revertFaces, bool skipNormals)
        {
            List<string> lines = new List<string>();

            if (!string.IsNullOrEmpty(source))
            {
                if (File.Exists(source))
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(source))
                        {
                            int lineIndex = 0;
                            int vIndex = 0;
                            int nIndex = 0;
                            int fIndex = 0;
                            string line;

                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line.StartsWith("v "))
                                {
                                    Vector3 point = useRelativeCoordinates ? diffs[vIndex++] : points[vIndex++];

                                    String vLine = String.Format(CultureInfo.InvariantCulture, "v {0} {1} {2}", point.X, rotateYZ ? point.Z : point.Y, rotateYZ ? -point.Y : point.Z);
                                    lines.Add(vLine);
                                }
                                else if (line.StartsWith("vn "))
                                {
                                    //It skips add a new line with normals
                                    if (!skipNormals)
                                    {
                                        Vector3 normal = normals[nIndex++];
                                        String nLine = String.Format(CultureInfo.InvariantCulture, "vn {0} {1} {2}", normal.X, rotateYZ ? normal.Z : normal.Y, rotateYZ ? -normal.Y : normal.Z);
                                        lines.Add(nLine);
                                    }
                                }
                                else if (revertFaces && line.StartsWith("f "))
                                {
                                    List<Tuple<int, int, int>> face = faces[fIndex++];

                                    List<String> faceLine = new List<string>();
                                    String faceItem = String.Empty;

                                    foreach (Tuple<int, int, int> item in face)
                                    {
                                        int f_vIndex = item.Item1;
                                        int f_tIndex = item.Item2;
                                        int f_nIndex = item.Item3;

                                        if (f_vIndex > 0 && f_tIndex > 0 && f_nIndex > 0)
                                        {
                                            faceItem = String.Format(CultureInfo.InvariantCulture, " {0}/{1}/{2}", f_vIndex, f_tIndex, f_nIndex);
                                        }
                                        else if (f_vIndex > 0 && f_tIndex == -1 && (f_nIndex == -1 || skipNormals))
                                        {
                                            faceItem = String.Format(CultureInfo.InvariantCulture, " {0}", f_vIndex);
                                        }
                                        else if (f_tIndex == -1)
                                        {
                                            faceItem = String.Format(CultureInfo.InvariantCulture, " {0}//{1}", f_vIndex, f_nIndex);
                                        }
                                        else if (f_vIndex > 0 && f_tIndex > 0 && (f_nIndex > 0))
                                        {
                                            faceItem = String.Format(CultureInfo.InvariantCulture, " {0}/{1}", f_vIndex, f_tIndex);
                                        }

                                        faceLine.Add(faceItem);
                                    }

                                    String fLine = String.Empty;
                                    StringBuilder sb = new StringBuilder("f");

                                    for (int i = faceLine.Count - 1; i >= 0; i--)
                                    {
                                        sb.Append(faceLine[i]);
                                    }

                                    lines.Add(sb.ToString());
                                }
                                else if (line.StartsWith("#")) //Comment
                                {
                                    if (lineIndex == 0 && line.StartsWith("#"))
                                    {
                                        lines.Add("# Created by SunMe [Peter Burian] v1.0");
                                    }
                                    else if (lineIndex == 1)
                                    { 
                                        string comment = String.Format(CultureInfo.InvariantCulture, "# Created {0}", DateTime.Now);
                                        lines.Add(comment);
                                    }
                                }
                                else
                                {
                                    lines.Add(line);
                                }
                                lineIndex++;
                            }
                        }
                        File.WriteAllLines(target, lines);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
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

        public bool CalculateDiffedVectors()
        {
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 diff = points[i] - center;
                diffs.Add(diff);
            }

            return diffs != null && diffs.Count == points.Count;
        }
    }
}
