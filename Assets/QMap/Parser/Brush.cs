using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace QMap
{
    public class Brush
    {
        private List<Face> faces = new List<Face>();
        public ReadOnlyCollection<Face> Faces { get { return faces.AsReadOnly(); } }
        public void AddFace(Face f) { faces.Add(f); }

        internal Brush() { }
    }
}
