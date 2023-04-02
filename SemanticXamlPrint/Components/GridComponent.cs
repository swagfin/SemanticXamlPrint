using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class GridComponent : IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        public Dictionary<string, string> CustomProperties { get; private set; } = new Dictionary<string, string>();
        //Component Attributes
        public int Rows { get; set; } = 0;
        public int Columns { get; set; } = 0;
        public bool TrySetProperty(string propertyName, string value)
        {
            try
            {
                switch (propertyName)
                {
                    case "rows":
                        Rows = Convert.ToInt32(value);
                        break;
                    case "columns":
                        Columns = Convert.ToInt32(value);
                        break;
                    default:
                        if (!CustomProperties.ContainsKey(propertyName)) CustomProperties.Add(propertyName, value);
                        break;
                }
                return true;
            }
            catch { return false; }
        }
        public void AddChild(IXamlComponent child)
        {
            Children.Add(child);
        }
    }
}
