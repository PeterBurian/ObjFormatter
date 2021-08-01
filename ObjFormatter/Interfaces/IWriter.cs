using System;
using System.Collections.Generic;
using System.Numerics;

namespace ObjFormatter.Interfaces
{
    public interface IWriter
    {
        void SetCenter(Vector3 center);

        void SetPoints(List<Vector3> points);

        void SetNormals(List<Vector3> normals);

        void SetFaces(List<List<Tuple<int, int, int>>> faces);

        bool CalculateDiffedVectors();

        void Write(string target);
    }
}
