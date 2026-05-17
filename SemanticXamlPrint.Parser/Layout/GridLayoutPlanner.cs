using SemanticXamlPrint.Parser.Components;
using System.Collections.Generic;
using System.Linq;

namespace SemanticXamlPrint.Parser.Layout
{
    public static class GridLayoutPlanner
    {
        public static GridLayoutPlan Build(GridComponent gridComponent, float maxLayoutWidth)
        {
            List<int> columnWidths = ColumnWidthCalculator.Calculate(gridComponent.ColumnWidths, maxLayoutWidth);
            List<GridRowComponent> gridRows = gridComponent.Children?.Where(element => element.Type == typeof(GridRowComponent)).Select(validElement => (GridRowComponent)validElement).ToList() ?? new List<GridRowComponent>();

            GridLayoutPlan plan = new GridLayoutPlan
            {
                ColumnWidths = columnWidths
            };

            foreach (GridRowComponent row in gridRows)
            {
                GridRowLayoutPlan rowPlan = new GridRowLayoutPlan
                {
                    Row = row
                };

                for (int colIndex = 0; colIndex < columnWidths.Count; colIndex++)
                {
                    IXamlComponent componentUnderColumn = row.Children?.FirstOrDefault(component => component.CustomProperties.IsPropertyExistsWithValue("grid.column", colIndex.ToString()));
                    rowPlan.ComponentsByColumn.Add(componentUnderColumn);
                }

                plan.Rows.Add(rowPlan);
            }

            return plan;
        }
    }
}
