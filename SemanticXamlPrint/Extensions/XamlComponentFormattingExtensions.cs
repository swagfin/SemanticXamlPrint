using SemanticXamlPrint.Parser.Components;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SemanticXamlPrint
{
    internal static class XamlComponentFormattingExtensions
    {
        public static ComponentDrawingFormatting GetSystemDrawingProperties(this IXamlComponent component, ComponentDrawingFormatting parentFormatting)
        {

            if (!component.Type.IsSubclassOf(typeof(XamlComponentCommonProperties))) return parentFormatting;
            XamlComponentCommonProperties styleFmt = (XamlComponentCommonProperties)component;
            //Return Custom
            return new ComponentDrawingFormatting
            {
                Font = new Font((string.IsNullOrEmpty(styleFmt.Font) ? parentFormatting.Font.Name : styleFmt.Font),
                                ((styleFmt.FontSize <= 0) ? parentFormatting.Font.Size : styleFmt.FontSize),
                                (string.IsNullOrEmpty(styleFmt.FontStyle) ? parentFormatting.Font.Style : GetOverridedFontStyle(styleFmt.FontStyle))),

                StringFormat = string.IsNullOrEmpty(styleFmt.Align) ? parentFormatting.StringFormat : GetConvertedStringFormat(styleFmt.Align),
                Brush = string.IsNullOrEmpty(styleFmt.Color) ? parentFormatting.Brush : GetSolidBrushFromColorString(styleFmt.Color)
            };
        }

        private static FontStyle GetOverridedFontStyle(string fontStyle)
        {
            switch (fontStyle?.Trim()?.ToLower())
            {
                case "bold":
                    return FontStyle.Bold;
                case "italic":
                    return FontStyle.Italic;
                case "underline":
                    return FontStyle.Underline;
                case "strikeout":
                    return FontStyle.Strikeout;
                default:
                    return FontStyle.Regular;
            }
        }
        private static StringFormat GetConvertedStringFormat(string alignment)
        {
            switch (alignment?.Trim()?.ToLower())
            {
                case "center":
                    return new StringFormat { Alignment = StringAlignment.Center };
                case "right":
                    return new StringFormat { Alignment = StringAlignment.Far };
                default:
                    return new StringFormat { Alignment = StringAlignment.Near };
            }
        }

        public static DashStyle ToDashStyle(this string style)
        {
            switch (style?.Trim()?.ToLower())
            {
                case "dash":
                    return DashStyle.Dash;
                case "dot":
                    return DashStyle.Dot;
                case "dashdot":
                    return DashStyle.DashDot;
                case "dashdotdot":
                    return DashStyle.DashDotDot;
                default:
                    return DashStyle.Solid;
            }
        }
        public static Brush GetSolidBrushFromColorString(string colorString)
        {
            if (string.IsNullOrEmpty(colorString)) return Brushes.Black;
            string colorCode = colorString.ToLower().Trim();
            //support html colors e.g. #B56E22
            if (colorCode.StartsWith("#") && colorCode.Length == 7) return GetHtmlColor(colorCode);
            switch (colorCode)
            {
                case "red":
                    return new SolidBrush(Color.Red);
                case "green":
                    return new SolidBrush(Color.Green);
                case "blue":
                    return new SolidBrush(Color.Blue);
                case "yellow":
                    return new SolidBrush(Color.Yellow);
                case "orange":
                    return new SolidBrush(Color.Orange);
                case "purple":
                    return new SolidBrush(Color.Purple);
                case "pink":
                    return new SolidBrush(Color.Pink);
                case "black":
                    return new SolidBrush(Color.Black);
                case "white":
                    return new SolidBrush(Color.White);
                case "gray":
                    return new SolidBrush(Color.Gray);
                case "brown":
                    return new SolidBrush(Color.Brown);
                case "cyan":
                    return new SolidBrush(Color.Cyan);
                case "magenta":
                    return new SolidBrush(Color.Magenta);
                // Add more cases for additional color names
                default:
                    return new SolidBrush(Color.Black);
            }
        }
        private static Brush GetHtmlColor(string colorString)
        {
            try { return new SolidBrush(ColorTranslator.FromHtml(colorString)); }
            catch { return new SolidBrush(Color.Black); }
        }
    }
    public class ComponentDrawingFormatting
    {
        public Font Font { get; set; }
        public StringFormat StringFormat { get; set; }
        public Brush Brush { get; set; }
    }
}
