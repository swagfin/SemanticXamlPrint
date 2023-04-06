using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class ImageComponent : IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        public List<XamlComponentCustomProperty> CustomProperties { get; private set; } = new List<XamlComponentCustomProperty>();
        //Component Attributes
        public string Source { get; set; } = null;
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public bool TrySetProperty(string propertyName, string value)
        {
            try
            {
                switch (propertyName)
                {
                    case "source":
                        Source = value?.Trim();
                        break;
                    case "width":
                        Width = (int.TryParse(value, out int width) && width > 0) ? width : 0;
                        break;
                    case "height":
                        Height = (int.TryParse(value, out int height) && height > 0) ? height : 0;
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
