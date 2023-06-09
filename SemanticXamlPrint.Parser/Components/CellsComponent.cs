﻿using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Parser.Components
{
    public class CellsComponent : XamlComponentCommonProperties, IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
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
        public void AddChild(IXamlComponent child)
        {
            if (child.Type != typeof(CellComponent)) throw new Exception($"[{Name}] can only contain child elements of type: [{nameof(CellComponent)}]");
            Children.Add(child);
        }
    }
}
