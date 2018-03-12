using System.Collections.Generic;
using UnityEngine;

namespace QMap
{
    public class Face
    {
        public Plane Plane { get; private set; }
        public Vector3 Normal { get { return Plane.normal; } }
        public float Distance { get { return Plane.distance; } }

        public string TextureName { get; private set; }
        public int XOffset { get; private set; }
        public int YOffset { get; private set; }
        public int Rotation { get; private set; }
        public float XScale { get; private set; }
        public float YScale { get; private set; }

        internal Face(Vector3 v1, Vector3 v2, Vector3 v3, string textureName, int xOffset, int yOffset, int rotation, float xScale, float yScale)
        {
            Plane = new Plane(v1, v2, v3);
            TextureName = textureName;
            XOffset = xOffset;
            YOffset = yOffset;
            Rotation = rotation;
            XScale = xScale;
            YScale = yScale;
        }
    }
}
