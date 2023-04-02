using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class TextBlockComponent : IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        public Dictionary<string, string> CustomProperties { get; private set; } = new Dictionary<string, string>();
        //Component Attributes
        public string Text { get; set; }
        public TextBlockComponent(string text, bool trimWhitespaces = true)
        {
            Text = trimWhitespaces ? text?.Trim() : text;
        }
        public bool TrySetProperty(string propertyName, string value)
        {
            if (!CustomProperties.ContainsKey(propertyName))
            {
                CustomProperties.Add(propertyName, value);
                return true;
            }
            return false;
        }
        public void AddChild(IXamlComponent child) { }
        //Override to String
        public override string ToString() => Text;
    }
}
