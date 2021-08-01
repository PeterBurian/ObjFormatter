using System;
using System.Collections.Generic;
using System.Numerics;

namespace ObjFormatter.Interfaces
{
    public interface IReader
    {
        string Source { get; }

        Vector3 Center { get; }

        List<Vector3> Points { get; }

        List<Vector3> Normals { get; }

        List<List<Tuple<int, int, int>>> Faces { get; }

        void Parse();
    }
}
