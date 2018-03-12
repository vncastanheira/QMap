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

        internal void CreatePolygon()
        {
            Polys = new Polygon[faces.Count];

            for (int i = 0; i < faces.Count - 1; i++)
            {
                for (int j = i; j < faces.Count - 1; j++)
                {
                    for (int k = j; k < faces.Count - 1; k++)
                    {
                        if (i != j && j != k && i != k)
                        {
                            Vector3 newVertex = Vector3.zero;
                            if (GetIntersection(
                                faces[i], faces[j], faces[k], 
                                Vector3.zero, Vector3.zero, Vector3.zero, 
                                out newVertex))
                            {
                                bool legal = true;

                                for (int m = 0; m < faces.Count -1; m++)
                                {
                                    // test if point if outside the brush
                                    if (Vector3.Dot(faces[m].Plane.normal, newVertex) + faces[m].Distance > 0)
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

            //if (GetIntersection)
            //{

            //}

        }

        internal bool GetIntersection(
            Face f1, Face f2, Face f3,
            Vector3 d1, Vector3 d2, Vector3 d3, out Vector3 vertex)
        {
            Vector3 n1 = f1.Normal;
            Vector3 n2 = f2.Normal;
            Vector3 n3 = f3.Normal;

            float denom = Vector3.Dot(n1, Vector3.Cross(n2, n3));
            if(denom == 0)
            {
                vertex = Vector3.zero;
                return false;
            }

            vertex =  -d1 * Vector3.Dot(n2, n3) - d2 * Vector3.Dot(n3, n1) - d3 * Vector3.Dot(n1, n2) / denom;
            return true;
        }
    }
}
