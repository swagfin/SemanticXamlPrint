using SemanticXamlPrint.Demo.SystemDrawing;
using System.Drawing;

namespace SemanticXamlPrint.Demo
{
    public static class Defaults
    {
        public static ComponentDrawingFontFormatting Formatting = new ComponentDrawingFontFormatting
        {
            Font = new Font("Calibri", 12, FontStyle.Regular),
            StringFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near },
            Brush = Brushes.Black,
        };
    }
}
