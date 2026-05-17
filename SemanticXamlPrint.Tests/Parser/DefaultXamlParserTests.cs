using SemanticXamlPrint.Parser;
using SemanticXamlPrint.Parser.Components;
using System.Text;

namespace SemanticXamlPrint.Tests.Parser
{
    public class DefaultXamlParserTests
    {
        [Fact]
        public void Parse_WithValidTemplate_ReturnsTemplateTree()
        {
            string xml = "<Template Font='Calibri' FontSize='10'><Data Align='Center'>Hello</Data></Template>";
            byte[] bytes = Encoding.UTF8.GetBytes(xml);

            IXamlComponent component = DefaultXamlParser.Parse(bytes);

            Assert.NotNull(component);
            Assert.Equal(typeof(TemplateComponent), component.Type);
            Assert.Single(component.Children);
            Assert.Equal(typeof(DataComponent), component.Children[0].Type);
            DataComponent dataComponent = (DataComponent)component.Children[0];
            Assert.Equal("Hello", dataComponent.Text);
            Assert.Equal("Center", dataComponent.Align);
        }

        [Fact]
        public void Parse_WithInvalidNode_ThrowsException()
        {
            string xml = "<Template><Unknown /></Template>";
            byte[] bytes = Encoding.UTF8.GetBytes(xml);

            Exception exception = Assert.Throws<Exception>(() => DefaultXamlParser.Parse(bytes));

            Assert.Contains("Invalid node name", exception.Message);
        }

        [Fact]
        public void Parse_WithGridMinHeight_MapsProperty()
        {
            string xml = "<Template><Grid ColumnWidths='1*1' MinHeight='240'><GridRow><Data Grid.Column='0'>A</Data><Data Grid.Column='1'>B</Data></GridRow></Grid></Template>";
            byte[] bytes = Encoding.UTF8.GetBytes(xml);

            IXamlComponent component = DefaultXamlParser.Parse(bytes);
            GridComponent gridComponent = (GridComponent)component.Children[0];

            Assert.Equal(240f, gridComponent.MinHeight);
        }

        [Fact]
        public void Parse_WithGridFillRemaining_MapsHeightModeAndBottomReserve()
        {
            string xml = "<Template><Grid ColumnWidths='1*1' HeightMode='FillRemaining' BottomReserve='48'><GridRow><Data Grid.Column='0'>A</Data><Data Grid.Column='1'>B</Data></GridRow></Grid></Template>";
            byte[] bytes = Encoding.UTF8.GetBytes(xml);

            IXamlComponent component = DefaultXamlParser.Parse(bytes);
            GridComponent gridComponent = (GridComponent)component.Children[0];

            Assert.Equal("FillRemaining", gridComponent.HeightMode);
            Assert.Equal(48f, gridComponent.BottomReserve);
        }
    }
}
