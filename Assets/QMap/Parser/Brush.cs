using QMap;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace QMap
{
    internal class Brush
    {
        private List<Face> faces = new List<Face>();
        internal ReadOnlyCollection<Face> Faces { get { return faces.AsReadOnly(); } }

        internal Polygon[] Polys;

        internal Brush() { }

        internal void AddFace(Face f) { faces.Add(f); }

        internal void CreatePolygons()
        {
            Polys = new Polygon[faces.Count];

            for (int i = 0; i < faces.Count; i++)
            {
                for (int j = i; j < faces.Count; j++)
                {
                    for (int k = j; k < faces.Count; k++)
                    {
                        if (i != j && j != k && i != k)
                        {
                            Vector3 newVertex = Vector3.zero;
                            if (GetIntersection(faces[i], faces[j], faces[k], out newVertex))
                            {
                                bool legal = true;

                                for (int m = 0; m < faces.Count; m++)
                                {
                                    // test if point is outside the brush
                                    if (Vector3.Dot(faces[m].Normal, newVertex) + faces[m].Distance > 0)
                                        legal = false;

                                }

                                if (legal)  // vertex is valid
                                {
                                    Polys[i].AddVertex(newVertex);
                                    Polys[j].AddVertex(newVertex);
                                    Polys[k].AddVertex(newVertex);
                                }
                            }

                        }

                    }
                }
            }
        }

        internal bool GetIntersection(Face f1, Face f2, Face f3, out Vector3 vertex)
        {
            Vector3 n1 = f1.Normal;
            Vector3 n2 = f2.Normal;
            Vector3 n3 = f3.Normal;

            float d1 = f1.Distance;
            float d2 = f2.Distance;
            float d3 = f3.Distance;

            float denom = Vector3.Dot(n1, Vector3.Cross(n2, n3));
            if (denom == 0)
            {
                vertex = Vector3.zero;
                return false;
            }

            vertex = (-d1 * Vector3.Cross(n2, n3)) - (d2 * Vector3.Cross(n3, n1)) - (d3 * Vector3.Cross(n1, n2)) / denom;
            return true;
        }
    }
}
