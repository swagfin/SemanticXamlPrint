using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class LineBreakComponent : IXamlComponent
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
        public void AddChild(IXamlComponent child) => throw new Exception($"property of type {nameof(LineComponent)} can not accept childrens");
    }

}
