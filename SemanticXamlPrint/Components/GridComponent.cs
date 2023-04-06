﻿using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class GridComponent : XamlComponentCommonProperties, IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        public string ColumnWidths { get; set; } = null;
        public string BorderStyle { get; set; }

        public bool TrySetProperty(string propertyName, string value)
        {
            try
            {
                if (base.SetCommonProperties(propertyName, value)) return true;
                switch (propertyName)
                {
                    case "columnwidths":
                        ColumnWidths = value.Contains("*") ? value?.Trim() : null;
                        break;
                    case "borderstyle":
                        BorderStyle = value?.Trim();
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
            if (child.Type != typeof(GridColumnComponent) && child.Type != typeof(GridRowComponent)) throw new Exception($"[{Name}] can only contain child elements of type: [{nameof(GridColumnComponent)}] or [{nameof(GridRowComponent)}]");
            Children.Add(child);
        }
    }
}
