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
        private List<Entity> entities = new List<Entity>();
        public List<Entity> Entities { get { return entities; } }

        public void AddEntity(Entity e)
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
                                    Vector3 v1 = new Vector3();
                                    Vector3 v2 = new Vector3();
                                    Vector3 v3 = new Vector3();
                                    v1.FromToken(tokenizer);
                                    v2.FromToken(tokenizer);
                                    v3.FromToken(tokenizer);

                                    string textureName = tokenizer.GetNextValue();
                                    int xOffset = Convert.ToInt32(tokenizer.GetNextValue());
                                    int yOffset = Convert.ToInt32(tokenizer.GetNextValue());
                                    int rotation = Convert.ToInt32(tokenizer.GetNextValue());
                                    float xScale = Convert.ToSingle(tokenizer.GetNextValue());
                                    float yScale = Convert.ToSingle(tokenizer.GetNextValue());
                                    b.AddFace(new Face(v1, v2, v3, textureName, xOffset, yOffset, rotation, xScale, yScale));
                                }
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

            Debug.Log("Created " + entities.Count + " entities.");
        }

        /// <summary>
        /// Create the visual representation of the map
        /// </summary>
        public void CreateMap()
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
        
    }
}
