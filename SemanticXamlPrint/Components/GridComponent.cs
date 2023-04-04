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
        public string ColumnWidths { get; set; } = null;
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
                    case "columnwidths":
                        ColumnWidths = value.Contains("*") ? value : null;
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
            if (child.Type != typeof(DataComponent)) throw new Exception($"[{Name}] can only contain child elements of type: [{nameof(DataComponent)}]");
            Children.Add(child);
        }
    }
}
