using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class TemplateComponent : XamlComponentCommonProperties, IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        //Component Attributes
        public int LineSpacing { get; set; } = 0;
        public int MaxWidth { get; set; } = 0;
        public int MarginTop { get; set; } = 0;
        public bool TrySetProperty(string propertyName, string value)
        {
            try
            {
                if (base.SetCommonProperties(propertyName, value)) return true;
                switch (propertyName)
                {
                    case "linespacing":
                        LineSpacing = Convert.ToInt32(value);
                        break;
                    case "maxwidth":
                        MaxWidth = Convert.ToInt32(value);
                        break;
                    case "margintop":
                        MarginTop = Convert.ToInt32(value);
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
