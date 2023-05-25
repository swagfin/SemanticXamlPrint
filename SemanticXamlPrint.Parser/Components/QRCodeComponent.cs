using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Parser.Components
{
    public class QRCodeComponent : IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        public List<XamlComponentCustomProperty> CustomProperties { get; private set; } = new List<XamlComponentCustomProperty>();
        //Component Attributes
        public string Text { get; set; }
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public bool TrySetProperty(string propertyName, string value)
        {
            try
            {
                switch (propertyName)
                {
                    case "text":
                        Text = value;
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
        public void AddChild(IXamlComponent child)
        {
            if (child?.Name == nameof(TextBlockComponent))
            {
                this.Text = (child.ToString()) ?? this.Text;
            }
        }
    }

}
