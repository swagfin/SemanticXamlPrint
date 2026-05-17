using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticXamlPrint.Parser.Layout
{
    public static class ColumnWidthCalculator
    {
        public static List<int> Calculate(string pattern, float maxLayoutWidth)
        {
            try
            {
                if (string.IsNullOrEmpty(pattern)) return Calculate(1, maxLayoutWidth);
                List<int> columnWidths = new List<int>();
                int total = pattern.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries).Sum(part => Convert.ToInt32(part));
                if (total < 1) return Calculate(1, maxLayoutWidth);
                int remainingWidth = (int)maxLayoutWidth;
                foreach (string part in pattern.Split('*'))
                {
                    int width = (int)Math.Round((double)remainingWidth / total * Convert.ToInt32(part));
                    columnWidths.Add(width);
                    remainingWidth -= width;
                    total -= Convert.ToInt32(part);
                }
                return columnWidths;
            }
            catch
            {
                return Calculate(1, maxLayoutWidth);
            }
        }

        public static List<int> Calculate(int columns, float maxLayoutWidth)
        {
            int safeColumns = columns <= 0 ? 1 : columns;
            int evenColumnWidth = (int)maxLayoutWidth / safeColumns;
            List<int> columnWidths = new List<int>();
            for (int index = 0; index < safeColumns; index += 1)
            {
                columnWidths.Add(evenColumnWidth);
            }
            return columnWidths;
        }
    }
}
