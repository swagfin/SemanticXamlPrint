using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class LineComponent : IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        public List<XamlComponentCustomProperty> CustomProperties { get; private set; } = new List<XamlComponentCustomProperty>();
        //Component Attributes
        public string Style { get; set; } = null;
        public bool TrySetProperty(string propertyName, string value)
        {
            try
            {
                switch (propertyName)
                {
                    case "style":
                        Style = value;
                        break;
                    default:
                        CustomProperties.AddCustomProperty(propertyName, value);
                        break;
                }
                return true;
            }
            catch { return false; }
        }
        public void AddChild(IXamlComponent child) => throw new Exception($"property of type {Name} can not accept childrens");
    }

}
