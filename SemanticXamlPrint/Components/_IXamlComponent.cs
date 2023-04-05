using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public interface IXamlComponent
    {
        string Name { get; }
        Type Type { get; }
        bool TrySetProperty(string propertyName, string value);
        void AddChild(IXamlComponent child);
        List<IXamlComponent> Children { get; }
        List<XamlComponentCustomProperty> CustomProperties { get; }
    }
}
