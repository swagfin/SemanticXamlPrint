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
        public double MaxWidth { get; set; } = 0;
        public double MarginTop { get; set; } = 0;
        public double MarginLeft { get; set; } = 0;
        public double MarginRight { get; set; } = 0;
        public double MarginBottom { get; set; } = 0;
        public bool TrySetProperty(string propertyName, string value)
        {
            try
            {
                if (base.SetCommonProperties(propertyName, value)) return true;
                switch (propertyName)
                {
                    case "maxwidth":
                        MaxWidth = (double.TryParse(value, out double maxWidth) && maxWidth > 0) ? maxWidth : 0;
                        break;
                    case "marginleft":
                        MarginLeft = (double.TryParse(value, out double marginLeft) && marginLeft > 0) ? marginLeft : 0;
                        break;
                    case "margintop":
                        MarginTop = (double.TryParse(value, out double marginTop) && marginTop > 0) ? marginTop : 0;
                        break;
                    case "marginright":
                        MarginRight = (double.TryParse(value, out double marginRight) && marginRight > 0) ? marginRight : 0;
                        break;
                    case "marginbottom":
                        MarginBottom = (double.TryParse(value, out double marginBottom) && marginBottom > 0) ? marginBottom : 0;
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