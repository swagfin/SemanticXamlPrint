using System.Collections.Generic;

namespace SemanticXamlPrint.Parser.Components
{
    public abstract class XamlComponentCommonProperties
    {
        public string Font { get; set; } = null;
        public string FontStyle { get; set; } = null;
        public int FontSize { get; set; } = 0;
        public string Align { get; set; } = null;
        public string Color { get; set; } = null;
        public float PaddingLeft { get; set; } = 0;
        public float PaddingTop { get; set; } = 0;
        public float PaddingRight { get; set; } = 0;
        public float PaddingBottom { get; set; } = 0;
        public string Background { get; set; } = null;
        public string BorderSides { get; set; } = null;
        public float BorderWidth { get; set; } = 0;
        public string BorderColor { get; set; } = null;
        public List<XamlComponentCustomProperty> CustomProperties { get; private set; } = new List<XamlComponentCustomProperty>();
        public bool SetCommonProperties(string propertyName, string value)
        {
            try
            {
                switch (propertyName)
                {
                    case "font":
                        Font = value;
                        break;
                    case "fontstyle":
                        FontStyle = value;
                        break;
                    case "fontweight":
                        FontStyle = value;
                        break;
                    case "fontsize":
                        FontSize = int.TryParse(value, out int fontSize) ? fontSize : 0;
                        break;
                    case "align":
                        Align = value;
                        break;
                    case "textalign":
                        Align = value;
                        break;
                    case "foreground":
                        Color = value;
                        break;
                    case "color":
                        Color = value;
                        break;
                    case "padding":
                        float padding = float.TryParse(value, out float allPadding) && allPadding > 0 ? allPadding : 0;
                        PaddingLeft = padding;
                        PaddingTop = padding;
                        PaddingRight = padding;
                        PaddingBottom = padding;
                        break;
                    case "paddingleft":
                        PaddingLeft = float.TryParse(value, out float paddingLeft) && paddingLeft > 0 ? paddingLeft : 0;
                        break;
                    case "paddingtop":
                        PaddingTop = float.TryParse(value, out float paddingTop) && paddingTop > 0 ? paddingTop : 0;
                        break;
                    case "paddingright":
                        PaddingRight = float.TryParse(value, out float paddingRight) && paddingRight > 0 ? paddingRight : 0;
                        break;
                    case "paddingbottom":
                        PaddingBottom = float.TryParse(value, out float paddingBottom) && paddingBottom > 0 ? paddingBottom : 0;
                        break;
                    case "background":
                    case "backgroundcolor":
                        Background = value;
                        break;
                    case "bordersides":
                    case "border":
                        BorderSides = value;
                        break;
                    case "borderwidth":
                        BorderWidth = float.TryParse(value, out float borderWidth) && borderWidth > 0 ? borderWidth : 0;
                        break;
                    case "bordercolor":
                        BorderColor = value;
                        break;
                    default:
                        return false;
                }
                return true;
            }
            catch { return false; }
        }
    }
    public class XamlComponentCustomProperty
    {
        public XamlComponentCustomProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get; }
        public string Value { get; }
    }
}
