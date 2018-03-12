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
        internal Vector3 Center
        {
            get
            {
                Vector3 center = Vector3.zero;
                foreach (var v in Vertices)
                    center += v;

                center /= Vertices.Count;
                return center;
            }
        }

        internal void AddVertex(Vector3 vertex)
        {
            if (Vertices == null)
                Vertices = new List<Vector3>();

            Vertices.Add(vertex);
        }

        /// <summary>
        /// Sort the vertices
        /// Not exactly what was on the document
        /// TODO: test this
        /// </summary>
        internal void SortVertices()
        {
            var comparer = new VertexComparer();
            comparer.center = Center;
            Vertices.Sort(comparer);
        }
    }

    internal class VertexComparer : IComparer<Vector3>
    {
        public Vector3 center;

        public int Compare(Vector3 x, Vector3 y)
        {
            var a = Vector3.Dot(center, x);
            var b = Vector3.Dot(center, y);

            if (a > b)
                return 1;

            if (a < b)
                return -1;

            return 0;
        }
    }
}
