using SemanticXamlPrint.Parser.Components;
using System.Drawing;
using System.Drawing.Printing;

namespace SemanticXamlPrint.Tests.Rendering
{
    public class DefaultXamlGraphicsPaginationTests
    {
        [Fact]
        public void DrawXamlComponent_WithSmallPage_RequestsMorePages()
        {
            TemplateComponent template = CreateTemplateWithLineBreaks(3);

            using (Bitmap bitmap = new Bitmap(200, 200))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                PrintPageEventArgs args = CreatePrintPageEventArgs(graphics, 13);

                _ = args.DrawXamlComponent(template);

                Assert.True(args.HasMorePages);
            }
        }

        [Fact]
        public void DrawXamlComponent_AfterCompletion_ResetsStateForSameGraphicsContext()
        {
            TemplateComponent template = CreateTemplateWithLineBreaks(3);

            using (Bitmap bitmap = new Bitmap(200, 200))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                PrintPageEventArgs firstPage = CreatePrintPageEventArgs(graphics, 13);
                _ = firstPage.DrawXamlComponent(template);
                Assert.True(firstPage.HasMorePages);

                PrintPageEventArgs secondPage = CreatePrintPageEventArgs(graphics, 100);
                _ = secondPage.DrawXamlComponent(template);
                Assert.False(secondPage.HasMorePages);

                PrintPageEventArgs thirdPage = CreatePrintPageEventArgs(graphics, 13);
                _ = thirdPage.DrawXamlComponent(template);
                Assert.True(thirdPage.HasMorePages);
            }
        }

        private static TemplateComponent CreateTemplateWithLineBreaks(int count)
        {
            TemplateComponent template = new TemplateComponent();
            _ = template.TrySetProperty("margintop", "10");
            _ = template.TrySetProperty("marginbottom", "0");
            for (int index = 0; index < count; index += 1)
            {
                template.AddChild(new LineBreakComponent());
            }
            return template;
        }

        private static PrintPageEventArgs CreatePrintPageEventArgs(Graphics graphics, int pageHeight)
        {
            PageSettings pageSettings = new PageSettings
            {
                PaperSize = new PaperSize("Test", 200, pageHeight)
            };
            Rectangle pageBounds = new Rectangle(0, 0, 200, pageHeight);
            Rectangle marginBounds = new Rectangle(0, 0, 200, pageHeight);
            return new PrintPageEventArgs(graphics, marginBounds, pageBounds, pageSettings);
        }
    }
}
