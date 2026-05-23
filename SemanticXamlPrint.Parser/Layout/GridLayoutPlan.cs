using SemanticXamlPrint.Parser.Components;
using System.Collections.Generic;

namespace SemanticXamlPrint.Parser.Layout
{
    public class GridLayoutPlan
    {
        public List<int> ColumnWidths { get; set; } = new List<int>();
        public List<GridRowLayoutPlan> Rows { get; set; } = new List<GridRowLayoutPlan>();
    }

    public class GridRowLayoutPlan
    {
        public GridRowComponent Row { get; set; }
        public List<IXamlComponent> ComponentsByColumn { get; set; } = new List<IXamlComponent>();
    }
}
