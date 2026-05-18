using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Parser.Components
{
    public class GridComponent : XamlComponentCommonProperties, IXamlComponent
    {
        public string Name => Type.Name;
        public Type Type => this.GetType();
        public List<IXamlComponent> Children { get; private set; } = new List<IXamlComponent>();
        public string ColumnWidths { get; set; } = null;
        public string BorderStyle { get; set; }
        public new double BorderWidth { get; set; } = 0.3;
        public float MinHeight { get; set; } = 0;
        public string HeightMode { get; set; } = null;
        public float BottomReserve { get; set; } = 0;
        public bool RepeatHeaderOnPageBreak { get; set; } = false;
        public int HeaderRows { get; set; } = 1;
        public bool TrySetProperty(string propertyName, string value)
        {
            try
            {
                switch (propertyName)
                {
                    case "columnwidths":
                        ColumnWidths = value.Contains("*") ? value?.Trim() : null;
                        break;
                    case "borderstyle":
                        BorderStyle = value?.Trim();
                        break;
                    case "borderwidth":
                        BorderWidth = (double.TryParse(value, out double width) && width > 0) ? width : 0.3;
                        break;
                    case "minheight":
                    case "height":
                        MinHeight = (float.TryParse(value, out float minHeight) && minHeight > 0) ? minHeight : 0;
                        break;
                    case "heightmode":
                        HeightMode = value?.Trim();
                        break;
                    case "bottomreserve":
                        BottomReserve = (float.TryParse(value, out float bottomReserve) && bottomReserve > 0) ? bottomReserve : 0;
                        break;
                    case "repeatheaderonpagebreak":
                        RepeatHeaderOnPageBreak = bool.TryParse(value, out bool repeatHeaderOnPageBreak) && repeatHeaderOnPageBreak;
                        break;
                    case "headerrows":
                        HeaderRows = (int.TryParse(value, out int headerRows) && headerRows > 0) ? headerRows : 1;
                        break;
                    default:
                        if (base.SetCommonProperties(propertyName, value)) return true;
                        CustomProperties.AddCustomProperty(propertyName, value);
                        break;
                }
                return true;
            }
            catch { return false; }
        }
        public void AddChild(IXamlComponent child)
        {
            if (child.Type != typeof(GridRowComponent)) throw new Exception($"[{Name}] can only contain child elements of type: [{nameof(GridRowComponent)}]");
            Children.Add(child);
        }
    }
}
