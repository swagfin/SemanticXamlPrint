using PdfSharp.Drawing;
using SemanticXamlPrint.Components;
using System;

namespace SemanticXamlPrint.PDF
{
    internal static class IXamlComponentExtensions
    {
        public static ComponentXDrawingFormatting GetPdfXDrawingProperties(this IXamlComponent component, ComponentXDrawingFormatting parentFormatting)
        {

            if (!component.Type.IsSubclassOf(typeof(XamlComponentCommonProperties))) return parentFormatting;
            XamlComponentCommonProperties styleFmt = (XamlComponentCommonProperties)component;
            //Return Custom
            return new ComponentXDrawingFormatting
            {
                Font = new XFont((string.IsNullOrEmpty(styleFmt.Font) ? parentFormatting.Font.Name : styleFmt.Font),
                                ((styleFmt.FontSize <= 0) ? parentFormatting.Font.Size : styleFmt.FontSize),
                                (string.IsNullOrEmpty(styleFmt.FontStyle) ? parentFormatting.Font.Style : GetOverridedFontStyle(styleFmt.FontStyle))),

                StringFormat = string.IsNullOrEmpty(styleFmt.Align) ? parentFormatting.StringFormat : GetConvertedStringFormat(styleFmt.Align),
                Brush = string.IsNullOrEmpty(styleFmt.Color) ? parentFormatting.Brush : GetXSolidBrushFromColorString(styleFmt.Color)
            };
        }

        private static XFontStyle GetOverridedFontStyle(string fontStyle)
        {
            switch (fontStyle?.Trim()?.ToLower())
            {
                case "bold":
                    return XFontStyle.Bold;
                case "italic":
                    return XFontStyle.Italic;
                case "underline":
                    return XFontStyle.Underline;
                case "strikeout":
                    return XFontStyle.Strikeout;
                default:
                    return XFontStyle.Regular;
            }
        }
        private static XStringFormat GetConvertedStringFormat(string alignment)
        {
            switch (alignment?.Trim()?.ToLower())
            {
                case "center":
                    return new XStringFormat { Alignment = XStringAlignment.Center };
                case "right":
                    return new XStringFormat { Alignment = XStringAlignment.Far };
                default:
                    return new XStringFormat { Alignment = XStringAlignment.Near };
            }
        }

        public static XDashStyle ToDashStyle(this string style)
        {
            switch (style?.Trim()?.ToLower())
            {
                case "dash":
                    return XDashStyle.Dash;
                case "dot":
                    return XDashStyle.Dot;
                case "dashdot":
                    return XDashStyle.DashDot;
                case "dashdotdot":
                    return XDashStyle.DashDotDot;
                default:
                    return XDashStyle.Solid;
            }
        }
        public static XSolidBrush GetXSolidBrushFromColorString(string colorString)
        {
            if (string.IsNullOrEmpty(colorString)) return XBrushes.Black;
            return Enum.TryParse(colorString?.Trim(), true, out XKnownColor xKnownColor) ? new XSolidBrush(XColor.FromKnownColor(xKnownColor)) : XBrushes.Black;
        }
    }
    public class ComponentXDrawingFormatting
    {
        public XStringFormat StringFormat { get; set; }
        public XFont Font { get; set; }
        public XBrush Brush { get; set; }
    }
}