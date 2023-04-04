using SemanticXamlPrint.Demo.SystemDrawing;
using System.Drawing;

namespace SemanticXamlPrint.Demo.Extensions
{
    public static class GraphicsExtensions
    {
        public static int DrawStringAndReturnHeight(this Graphics graphics, string text, bool textWrap, ComponentDrawingFormatting cellFmt, float x, float y, float z, int lineSpacing)
        {
            if (textWrap && (int)graphics.MeasureString(text, cellFmt.Font).Width > z)
            {
                SizeF size = graphics.MeasureString(text, cellFmt.Font, (int)z);
                RectangleF layoutF = new RectangleF(new PointF(x, y), size);
                graphics.DrawString(text, cellFmt.Font, cellFmt.Brush, layoutF, cellFmt.StringFormat);
                return (int)layoutF.Height;
            }
            else
            {
                Rectangle layout = new Rectangle((int)x, (int)y, (int)z, (int)cellFmt.Font.Size + lineSpacing);
                graphics.DrawString(text, cellFmt.Font, cellFmt.Brush, layout, cellFmt.StringFormat);
                return layout.Height + lineSpacing;
            }
        }
    }
}
