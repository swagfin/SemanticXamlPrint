using SemanticXamlPrint.Parser.Components;
using System.Drawing;
using System.Drawing.Printing;

namespace SemanticXamlPrint.Tests.Rendering
{
    public class GridMinHeightRenderingTests
    {
        [Fact]
        public void DrawXamlComponent_WithGridMinHeight_UsesMinimumHeight()
        {
            TemplateComponent template = new TemplateComponent();
            _ = template.TrySetProperty("margintop", "10");
            _ = template.TrySetProperty("marginbottom", "0");

            GridComponent grid = new GridComponent();
            _ = grid.TrySetProperty("columnwidths", "1*1");
            _ = grid.TrySetProperty("borderstyle", "solid");
            _ = grid.TrySetProperty("minheight", "200");

            GridRowComponent row = new GridRowComponent();
            DataComponent left = new DataComponent();
            _ = left.TrySetProperty("grid.column", "0");
            left.Text = "Short";
            DataComponent right = new DataComponent();
            _ = right.TrySetProperty("grid.column", "1");
            right.Text = "Row";
            row.AddChild(left);
            row.AddChild(right);
            grid.AddChild(row);
            template.AddChild(grid);

            using (Bitmap bitmap = new Bitmap(400, 400))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                PrintPageEventArgs args = CreatePrintPageEventArgs(graphics, 1000);
                float finalY = args.DrawXamlComponent(template);

                Assert.True(finalY >= 210f);
            }
        }

        [Fact]
        public void DrawXamlComponent_WithGridFillRemaining_UsesPageBottomMinusReserve()
        {
            TemplateComponent template = new TemplateComponent();
            _ = template.TrySetProperty("margintop", "10");
            _ = template.TrySetProperty("marginbottom", "0");

            GridComponent grid = new GridComponent();
            _ = grid.TrySetProperty("columnwidths", "1*1");
            _ = grid.TrySetProperty("borderstyle", "solid");
            _ = grid.TrySetProperty("heightmode", "fillremaining");
            _ = grid.TrySetProperty("bottomreserve", "60");

            GridRowComponent row = new GridRowComponent();
            DataComponent left = new DataComponent();
            _ = left.TrySetProperty("grid.column", "0");
            left.Text = "Short";
            DataComponent right = new DataComponent();
            _ = right.TrySetProperty("grid.column", "1");
            right.Text = "Row";
            row.AddChild(left);
            row.AddChild(right);
            grid.AddChild(row);
            template.AddChild(grid);

            using (Bitmap bitmap = new Bitmap(400, 400))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                PrintPageEventArgs args = CreatePrintPageEventArgs(graphics, 500);
                float finalY = args.DrawXamlComponent(template);

                // margintop 10 + (pageBottom 500 - reserve 60 - startY 10) => finalY around 440
                Assert.True(finalY >= 430f);
                Assert.True(finalY <= 450f);
            }
        }

        private static PrintPageEventArgs CreatePrintPageEventArgs(Graphics graphics, int pageHeight)
        {
            PageSettings pageSettings = new PageSettings
            {
                PaperSize = new PaperSize("Test", 400, pageHeight)
            };
            Rectangle pageBounds = new Rectangle(0, 0, 400, pageHeight);
            Rectangle marginBounds = new Rectangle(0, 0, 400, pageHeight);
            return new PrintPageEventArgs(graphics, marginBounds, pageBounds, pageSettings);
        }
    }
}
