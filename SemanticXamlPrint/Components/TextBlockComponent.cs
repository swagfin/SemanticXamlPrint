using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class TextBlockComponent : XamlComponentCommonProperties, IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        //Component Attributes
        public string Text { get; set; }
        public TextBlockComponent(string text, bool trimWhitespaces = true)
        {
            Text = trimWhitespaces ? text?.Trim() : text;
        }
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
        public void AddChild(IXamlComponent child) => throw new Exception($"property of type {Name} can not accept childrens");
        //Override to String
        public override string ToString() => Text;
    }
}
