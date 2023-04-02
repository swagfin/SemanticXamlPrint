using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class DataRowComponent : IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        public Dictionary<string, string> CustomProperties { get; private set; } = new Dictionary<string, string>();

        public bool TrySetProperty(string propertyName, string value)
        {
            if (!CustomProperties.ContainsKey(propertyName))
            {
                CustomProperties.Add(propertyName, value);
                return true;
            }
            return false;
        }
        public void AddChild(IXamlComponent child)
        {
            if (child.Type != typeof(DataRowCellComponent)) throw new Exception($"[{Name}] can only contain child elements of type: [{nameof(DataRowCellComponent)}]");
            Children.Add(child);
        }
    }
}
