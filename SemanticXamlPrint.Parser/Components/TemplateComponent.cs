using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Parser.Components
{
    public class TemplateComponent : XamlComponentCommonProperties, IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        //Component Attributes
        public float MarginTop { get; set; } = 10;
        public float MarginLeft { get; set; } = 0;
        public float MarginRight { get; set; } = 0;
        public float MarginBottom { get; set; } = 0;
        public bool TrySetProperty(string propertyName, string value)
        {
            try
            {
                if (base.SetCommonProperties(propertyName, value)) return true;
                switch (propertyName)
                {
                    case "marginleft":
                        MarginLeft = (float.TryParse(value, out float marginLeft) && marginLeft > 0) ? marginLeft : 0;
                        break;
                    case "margintop":
                        MarginTop = (float.TryParse(value, out float marginTop) && marginTop > 0) ? marginTop : 0;
                        break;
                    case "marginright":
                        MarginRight = (float.TryParse(value, out float marginRight) && marginRight > 0) ? marginRight : 0;
                        break;
                    case "marginbottom":
                        MarginBottom = (float.TryParse(value, out float marginBottom) && marginBottom > 0) ? marginBottom : 0;
                        break;
                    case "document":
                        if (value.Equals("A4", StringComparison.CurrentCultureIgnoreCase))
                        {
                            this.MarginTop = 100;
                            this.MarginBottom = 100;
                            this.MarginLeft = 50;
                            this.MarginRight = 50;
                        }
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
            if (child?.Name == nameof(TemplateComponent)) throw new Exception("template can not have another template as its children");
            Children.Add(child);
        }
    }
}