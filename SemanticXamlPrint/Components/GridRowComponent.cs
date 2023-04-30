using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class GridRowComponent : XamlComponentCommonProperties, IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        public bool TrySetProperty(string propertyName, string value)
        {
            try
            {
                if (base.SetCommonProperties(propertyName, value)) return true;
                switch (propertyName)
                {
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
            Children.Add(child);
        }
    }
}
