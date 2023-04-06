using SemanticXamlPrint.Demo.SystemDrawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace SemanticXamlPrint.Demo.Extensions
{
    public static class GraphicsExtensions
    {
        public static int DrawStringAndReturnHeight(this Graphics graphics, string text, bool textWrap, ComponentDrawingFormatting cellFmt, float x, float y, float z)
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
                SizeF size = graphics.MeasureString(text, cellFmt.Font, (int)z, new StringFormat { FormatFlags = StringFormatFlags.NoWrap });
                Rectangle layout = new Rectangle((int)x, (int)y, (int)z, (int)size.Height);
                graphics.DrawString(text, cellFmt.Font, cellFmt.Brush, layout, cellFmt.StringFormat);
                return layout.Height;
            }
        }


        public static List<int> GetDivideColumnWidths(this Graphics graphics, string pattern, int columnsCount = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(pattern)) return GetDivideColumnWidths(graphics, columnsCount);
                List<int> columnWidths = new List<int>();
                int total = pattern.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries).Sum(p => Convert.ToInt32(p));
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

        public static int DrawlLineAndReturnHeight(this Graphics graphics, DashStyle dashStyle, float x, float y, float z)
        {
            using (Pen pen = new Pen(Color.Black) { DashStyle = dashStyle })
            {
                graphics.DrawLine(pen, x, y, z, y);
            }
            return 1;
        }

        public static int DrawImageCenteredAndReturnHeight(this Graphics graphics, Image image, float x, float y, float maxWidth = 0, float maxHeight = 0)
        {

            float newWidth = Math.Min(image.Height, maxWidth > 0 ? maxWidth : image.Width);
            float newHeight = Math.Min(image.Height, maxHeight > 0 ? maxHeight : image.Height);
            //Center Image
            float centeredX = x + (graphics.VisibleClipBounds.Width - newWidth) / 2;
            //Draw Image
            graphics.DrawImage(image, centeredX > 0 ? centeredX : x, y, newWidth, newHeight);
            return (int)newHeight;
        }
    }
}
