using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public void CreateMap(Tokenizer tokenizer)
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
    }
}
