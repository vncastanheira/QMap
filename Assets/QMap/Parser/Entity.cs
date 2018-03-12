using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace QMap
{
    internal class Entity
    {
        private List<Brush> brushes = new List<Brush>();
        private Dictionary<string, string> keyvals = new Dictionary<string, string>();

        internal ReadOnlyCollection<Brush> Brushes { get { return brushes.AsReadOnly(); } }
        internal Dictionary<string, string> Properties { get { return keyvals; } }

        internal void AddBrush(Brush b) { brushes.Add(b); }

        /// <summary> Check if the entity is the world map </summary>
        internal bool IsWorld()
        {
            string classname = string.Empty;
            if (keyvals.TryGetValue("classname", out classname))
            {
                return classname.Equals("worldspawn");
            }
            return false;
        }

        internal string this[string key]
        {
            get { string temp = ""; keyvals.TryGetValue(key, out temp); return temp; }
            set { keyvals[key] = value; }
        }

        internal Entity() { }
    }
}
