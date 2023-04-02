using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class LineComponent : IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        public Dictionary<string, string> CustomProperties { get; private set; } = new Dictionary<string, string>();
        //Component Attributes
        public bool IsDotted { get; set; } = true;
        public bool TrySetProperty(string propertyName, string value)
        {
            try
            {
                switch (propertyName)
                {
                    case "text":
                        IsDotted = Convert.ToBoolean(value);
                        break;
                    default:
                        if (!CustomProperties.ContainsKey(propertyName)) CustomProperties.Add(propertyName, value);
                        break;
                }
                return true;
            }
            catch { return false; }
        }
        public void AddChild(IXamlComponent child) => throw new Exception($"property of type {nameof(LineComponent)} can not accept childrens");
    }

}
