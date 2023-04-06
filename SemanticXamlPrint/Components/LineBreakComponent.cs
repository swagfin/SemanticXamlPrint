using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class LineBreakComponent : IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        public List<XamlComponentCustomProperty> CustomProperties { get; private set; } = new List<XamlComponentCustomProperty>();
        public bool TrySetProperty(string propertyName, string value)
        {
            try
            {
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
        public void AddChild(IXamlComponent child) => throw new Exception($"property of type {nameof(LineComponent)} can not accept childrens");
    }

}
