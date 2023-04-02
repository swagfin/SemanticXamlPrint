﻿using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Components
{
    public class DataRowCellComponent : XamlComponentCommonProperties, IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        public Dictionary<string, string> CustomProperties { get; private set; } = new Dictionary<string, string>();
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
                        TextWrap = Convert.ToBoolean(value);
                        break;
                    case "x":
                        X = float.Parse(value);
                        break;
                    case "y":
                        Y = float.Parse(value);
                        break;
                    case "z":
                        Z = float.Parse(value);
                        break;
                    default:
                        if (!CustomProperties.ContainsKey(propertyName)) CustomProperties.Add(propertyName, value);
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