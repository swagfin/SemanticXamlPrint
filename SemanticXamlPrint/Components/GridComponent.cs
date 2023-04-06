using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class GridComponent : XamlComponentCommonProperties, IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        public int Rows { get; set; } = 0;
        public int Columns { get; set; } = 0;
        public string ColumnWidths { get; set; } = null;
        public bool TrySetProperty(string propertyName, string value)
        {
            try
            {
                if (base.SetCommonProperties(propertyName, value)) return true;
                switch (propertyName)
                {
                    case "rows":
                        Rows = (int.TryParse(value, out int rows) && rows > 0) ? rows : 0;
                        break;
                    case "columns":
                        Columns = (int.TryParse(value, out int column) && column > 0) ? column : 0;
                        break;
                    case "columnwidths":
                        ColumnWidths = value.Contains("*") ? value?.Trim() : null;
                        break;
                    default:
                        CustomProperties.AddCustomProperty(propertyName, value);
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
