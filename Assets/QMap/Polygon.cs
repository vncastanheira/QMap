using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QMap
{
    [Serializable]
    internal struct Polygon
    {
        internal List<Vector3> Vertices { get; private set; }
        
        internal void AddVertex(Vector3 vertex)
        {
            if (Vertices == null)
                Vertices = new List<Vector3>();

            Vertices.Add(vertex);
        }
    }
}
