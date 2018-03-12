using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QMap
{
    public class Map : MonoBehaviour
    {
        Texture error;

        private List<Entity> entities = new List<Entity>();
        internal List<Entity> Entities { get { return entities; } }

        internal void AddEntity(Entity e)
        {
            entities.Add(e);
        }

        public void ReadToken(Tokenizer tokenizer)
        {
            while (tokenizer.PeekNextToken().Type != Tokenizer.TokenType.EndOfStream)
            {
                var token = tokenizer.GetNextToken();
                if (token.Type == Tokenizer.TokenType.StartBlock)
                {
                    Entity e = new Entity();
                    token = tokenizer.GetNextToken();
                    while (token.Type != Tokenizer.TokenType.EndBlock)
                    {
                        switch (token.Type)
                        {
                            case Tokenizer.TokenType.Value: // Key/value pair
                                var value = tokenizer.GetNextToken();
                                if (value.Type == Tokenizer.TokenType.Value)
                                {
                                    e[token.Contents] = value.Contents;
                                }
                                else
                                {
                                    throw new FormatException(String.Format("Expected a value, received a {0}", value));
                                }
                                break;
                            case Tokenizer.TokenType.StartBlock: // Brush
                                Brush b = new Brush();
                                while (tokenizer.PeekNextToken().Type != Tokenizer.TokenType.EndBlock)
                                {
                                    Vector3 v1 = Vector3Extension.FromToken(tokenizer);
                                    Vector3 v2 = Vector3Extension.FromToken(tokenizer);
                                    Vector3 v3 = Vector3Extension.FromToken(tokenizer);

                                    string textureName = tokenizer.GetNextValue();
                                    int xOffset = Convert.ToInt32(tokenizer.GetNextValue());
                                    int yOffset = Convert.ToInt32(tokenizer.GetNextValue());
                                    int rotation = Convert.ToInt32(tokenizer.GetNextValue());
                                    float xScale = Convert.ToSingle(tokenizer.GetNextValue());
                                    float yScale = Convert.ToSingle(tokenizer.GetNextValue());
                                    var newFace = new Face(v1, v2, v3, textureName, xOffset, yOffset, rotation, xScale, yScale);
                                    b.AddFace(newFace);
                                }
                                b.CreatePolygons();
                                tokenizer.GetNextToken(); // Brush end block
                                e.AddBrush(b);
                                break;
                            default:
                                throw new FormatException(String.Format("Expected either a block start or a value, received a {0}", token));
                        }
                        token = tokenizer.GetNextToken();
                    }
                    AddEntity(e);
                }
            }

            Debug.Log("Map created.");
            Debug.Log(" -" + entities.Count + " entities.");
            Debug.Log(" -" + entities[0].Brushes.Count + " brushes.");
        }

        /// <summary>
        /// Create the visual representation of the map
        /// </summary>
        internal void CreateMap()
        {
            // create the worldspawn map
            var world = entities.SingleOrDefault(e => e.IsWorld());
            if (world == null)
            {
                Debug.LogWarning("worldspawn class could not be found.");
                return;
            }
            
            // loop all world brushes
            int brushIndex = 0;
            foreach (var brush in world.Brushes)
            {
                Debug.Log("Creating brush " + brushIndex + "\nFaces: " + brush.Faces.Count);

                GameObject brushGO = new GameObject("brush " + brushIndex);
                MeshFilter brushMF = brushGO.AddComponent<MeshFilter>();
                MeshRenderer brushMR = brushGO.AddComponent<MeshRenderer>();
                brushGO.transform.SetParent(transform);
                brushGO.transform.SetSiblingIndex(brushIndex);

                var mesh = new Mesh();

                List<Vector3> vertices = new List<Vector3>();
                List<Vector2> uv = new List<Vector2>();

                int ti = 0;
                int submesh = 0;
                foreach (var face in brush.Faces)
                {

                    uv.Add(new Vector2(face.XOffset, face.YOffset));
                    //                    mesh.Clear();
                }
                try
                {
                    mesh.SetVertices(vertices);
                    mesh.SetUVs(0, uv);
                }
                catch (UnityEngine.UnityException ex)
                {
                    Debug.LogError(ex);
                }

                mesh.RecalculateNormals();
                brushMF.mesh = brushMF.sharedMesh = mesh;

                brushIndex++;
            }
        }
        
        internal void DrawPlane(Plane plane, float size = 1f)
        {
            Vector3 position = (size * plane.distance * plane.normal);
            Vector3 v3;
            if(plane.normal.normalized != Vector3.forward)
                v3 = Vector3.Cross(plane.normal, Vector3.forward).normalized * plane.normal.magnitude;
            else
                v3 = Vector3.Cross(plane.normal, Vector3.up).normalized * plane.normal.magnitude; ;

            var corner0 = position + v3;
            var corner2 = position - v3;
            var q = Quaternion.AngleAxis(90f, plane.normal);
            var corner1 = position + v3;
            var corner3 = position - v3;
            Debug.DrawLine(corner0, corner2, Color.green);
            Debug.DrawLine(corner1, corner3, Color.green);
            Debug.DrawLine(corner0, corner1, Color.green);
            Debug.DrawLine(corner1, corner2, Color.green);
            Debug.DrawLine(corner2, corner3, Color.green);
            Debug.DrawLine(corner3, corner0, Color.green);
            Debug.DrawRay(position, plane.normal, Color.red);
        }

        private void OnDrawGizmos()
        {
            foreach (var e in entities)
            {
                foreach (var brush in e.Brushes)
                {
                    foreach (var face in brush.Faces)
                    {
//                        DrawPlane(face.Plane, 2f);
                    }

                    if (brush.Polys == null)
                        return;

                    float c = 1.0f;
                    foreach (var p in brush.Polys)
                    {
                        if (p.Vertices != null)
                        {
                            Vector3 prev = Vector3.zero;
                            int i = 0;
                            foreach (var v in p.Vertices)
                            {
                                Gizmos.DrawSphere(v, 2f);
                                if (i > 0)
                                    Gizmos.DrawLine(prev, v);

                                prev = v;
                                i++;
                            }
                            if(i > 0)
                                Gizmos.DrawLine(prev, p.Vertices[0]);
                        }
                    }
                }
            }
        }
    }
}
