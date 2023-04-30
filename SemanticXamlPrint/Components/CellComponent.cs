using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class CellComponent : XamlComponentCommonProperties, IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        //Component Attributes
        public string Text { get; set; }
        public bool TextWrap { get; set; } = false;
        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
        public float Z { get; set; } = 0;
        public bool TrySetProperty(string propertyName, string value)
        {
            try
            {
                if (base.SetCommonProperties(propertyName, value)) return true;
                switch (propertyName)
                {
                    case "text":
                        Text = value;
                        break;
                    case "textwrap":
                        TextWrap = bool.TryParse(value, out bool wrap) && wrap;
                        break;
                    case "x":
                        X = (float.TryParse(value, out float x) && x > 0) ? x : 0;
                        break;
                    case "y":
                        Y = (float.TryParse(value, out float y) && y > 0) ? y : 0; ;
                        break;
                    case "z":
                        Z = (float.TryParse(value, out float z) && z > 0) ? z : 0; ;
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
