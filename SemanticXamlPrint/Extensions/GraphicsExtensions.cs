using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace SemanticXamlPrint
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


        public static List<int> GetDivideColumnWidths(this Graphics graphics, string pattern)
        {
            try
            {
                if (string.IsNullOrEmpty(pattern)) return GetDivideColumnWidths(graphics, 1);
                List<int> columnWidths = new List<int>();
                int total = pattern.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries).Sum(p => Convert.ToInt32(p));
                if (total < 1) return GetDivideColumnWidths(graphics, 1);
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
            catch { return GetDivideColumnWidths(graphics, 1); }
        }
        public static List<int> GetDivideColumnWidths(this Graphics graphics, int columns)
        {
            columns = columns <= 0 ? 1 : columns;
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
            float centeredX = x + (graphics.VisibleClipBounds.Width - newWidth) / 2;
            graphics.DrawImage(image, centeredX > 0 ? centeredX : x, y, newWidth, newHeight);
            return (int)newHeight;
        }
        public static int DrawRectangleAndReturnHeight(this Graphics graphics, DashStyle dashStyle, float x, float y, float z, float height)
        {

            using (Pen pen = new Pen(Color.Black) { DashStyle = dashStyle })
            {
                graphics.DrawRectangle(pen, x, y, z, height);
            }
            return (int)height;
        }
    }
}
