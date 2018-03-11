using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace QMap
{
    public class Entity
    {
        private List<Brush> brushes = new List<Brush>();
        private Dictionary<string, string> keyvals = new Dictionary<string, string>();

        public ReadOnlyCollection<Brush> Brushes { get { return brushes.AsReadOnly(); } }
        public Dictionary<string, string> KeyVals { get { return keyvals; } }
        public void AddBrush(Brush b) { brushes.Add(b); }

        public string this[string key]
        {
            get { string temp = ""; keyvals.TryGetValue(key, out temp); return temp; }
            set { keyvals[key] = value; }
        }

        internal Entity() { }
    }
}
