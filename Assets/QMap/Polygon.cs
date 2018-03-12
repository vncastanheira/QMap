using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QMap
{
    struct Polygon
    {
        List<Vector3> Vertices;

        public static Polygon Create()
        {
            var p = new Polygon();
            p.Vertices = new List<Vector3>();
            return p;
        }

        public void AddVertex(Vector3 vertex)
        {
            Vertices.Add(vertex);
        }
    }
}
