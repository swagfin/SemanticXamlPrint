using SemanticXamlPrint.Parser.Components;
using SemanticXamlPrint.Parser.Layout;

namespace SemanticXamlPrint.Tests.Layout
{
    public class GridLayoutPlannerTests
    {
        [Fact]
        public void Build_WithColumnPattern_CreatesExpectedColumnCount()
        {
            GridComponent gridComponent = new GridComponent();
            _ = gridComponent.TrySetProperty("columnwidths", "1*4*2");

            GridRowComponent row = new GridRowComponent();

            DataComponent first = new DataComponent();
            _ = first.TrySetProperty("grid.column", "0");
            first.Text = "A";

            DataComponent second = new DataComponent();
            _ = second.TrySetProperty("grid.column", "1");
            second.Text = "B";

            DataComponent third = new DataComponent();
            _ = third.TrySetProperty("grid.column", "2");
            third.Text = "C";

            row.AddChild(first);
            row.AddChild(second);
            row.AddChild(third);
            gridComponent.AddChild(row);

            GridLayoutPlan plan = GridLayoutPlanner.Build(gridComponent, 350f);

            Assert.Equal(3, plan.ColumnWidths.Count);
            Assert.Single(plan.Rows);
            Assert.Equal(first, plan.Rows[0].ComponentsByColumn[0]);
            Assert.Equal(second, plan.Rows[0].ComponentsByColumn[1]);
            Assert.Equal(third, plan.Rows[0].ComponentsByColumn[2]);
        }

        [Fact]
        public void Build_WithMissingColumn_KeepsNullPlaceholder()
        {
            GridComponent gridComponent = new GridComponent();
            _ = gridComponent.TrySetProperty("columnwidths", "1*1*1");

            GridRowComponent row = new GridRowComponent();

            DataComponent first = new DataComponent();
            _ = first.TrySetProperty("grid.column", "0");

            DataComponent third = new DataComponent();
            _ = third.TrySetProperty("grid.column", "2");

            row.AddChild(first);
            row.AddChild(third);
            gridComponent.AddChild(row);

            GridLayoutPlan plan = GridLayoutPlanner.Build(gridComponent, 300f);

            Assert.Equal(3, plan.Rows[0].ComponentsByColumn.Count);
            Assert.Equal(first, plan.Rows[0].ComponentsByColumn[0]);
            Assert.Null(plan.Rows[0].ComponentsByColumn[1]);
            Assert.Equal(third, plan.Rows[0].ComponentsByColumn[2]);
        }

        [Fact]
        public void Build_WithDuplicateColumn_UsesFirstComponentForThatColumn()
        {
            GridComponent gridComponent = new GridComponent();
            _ = gridComponent.TrySetProperty("columnwidths", "1*1");

            GridRowComponent row = new GridRowComponent();

            DataComponent firstAtColumnZero = new DataComponent();
            firstAtColumnZero.Text = "First";
            _ = firstAtColumnZero.TrySetProperty("grid.column", "0");

            DataComponent secondAtColumnZero = new DataComponent();
            secondAtColumnZero.Text = "Second";
            _ = secondAtColumnZero.TrySetProperty("grid.column", "0");

            DataComponent atColumnOne = new DataComponent();
            atColumnOne.Text = "Third";
            _ = atColumnOne.TrySetProperty("grid.column", "1");

            row.AddChild(firstAtColumnZero);
            row.AddChild(secondAtColumnZero);
            row.AddChild(atColumnOne);
            gridComponent.AddChild(row);

            GridLayoutPlan plan = GridLayoutPlanner.Build(gridComponent, 200f);

            Assert.Equal(firstAtColumnZero, plan.Rows[0].ComponentsByColumn[0]);
            Assert.Equal(atColumnOne, plan.Rows[0].ComponentsByColumn[1]);
        }
    }
}
