using SemanticXamlPrint.Components;
using SemanticXamlPrint.Components.CommonProperties;
using SemanticXamlPrint.Demo.SystemDrawing;
using System.Drawing;

namespace SemanticXamlPrint.Demo
{
    public static class IXamlComponentExtensions
    {
        public static ComponentDrawingFontFormatting GetTextFormattingProperties(this IXamlComponent component, ComponentDrawingFontFormatting parentFormatting)
        {

            if (!component.Type.IsSubclassOf(typeof(TextFormattingProperties))) return parentFormatting;
            TextFormattingProperties styleFmt = (TextFormattingProperties)component;
            //Return Custom
            return new ComponentDrawingFontFormatting
            {
                Font = new Font((string.IsNullOrEmpty(styleFmt.Font) ? parentFormatting.Font.Name : styleFmt.Font),
                                ((styleFmt.FontSize <= 0) ? parentFormatting.Font.Size : styleFmt.FontSize),
                                (string.IsNullOrEmpty(styleFmt.FontStyle) ? parentFormatting.Font.Style : GetOverridedFontStyle(styleFmt.FontStyle))),

                StringFormat = string.IsNullOrEmpty(styleFmt.Align) ? parentFormatting.StringFormat : GetConvertedStringFormat(styleFmt.Align),
                Brush = Brushes.Black,
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
                    return new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                case "right":
                    return new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far };
                default:
                    return new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
            }
        }
    }
}
