using SemanticXamlPrint.Demo.SystemDrawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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


        public static List<int> GetDivideColumnWidths(this Graphics graphics, string pattern, int columnsCount = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(pattern)) return GetDivideColumnWidths(graphics, columnsCount);
                List<int> columnWidths = new List<int>();
                int total = pattern.Split('*').Sum(p => Convert.ToInt32(p));
                if (total < columnsCount) return GetDivideColumnWidths(graphics, columnsCount);
                int remainingWidth = (int)graphics.VisibleClipBounds.Width;
                foreach (string s in pattern.Split('*'))
                {
                    int w = (int)Math.Round((double)remainingWidth / total * Convert.ToInt32(s));
                    columnWidths.Add(w);
                    remainingWidth -= w;
                    total -= Convert.ToInt32(s);
                }
                return columnWidths;
            }
            catch { return GetDivideColumnWidths(graphics, columnsCount); }
        }
        public static List<int> GetDivideColumnWidths(this Graphics graphics, int columns)
        {
            int evenColumnWidth = (int)graphics.VisibleClipBounds.Width / columns;
            List<int> columnWidths = new List<int>();
            for (var i = 0; i < columns; i += 1)
                columnWidths.Add(evenColumnWidth);
            return columnWidths;
        }
    }
}
