using SemanticXamlPrint.Parser.Components;
using System.Collections.Generic;

namespace SemanticXamlPrint.Tests.Rendering
{
    public class CustomPropertyExtensionsTests
    {
        [Fact]
        public void AddCustomProperty_IsCaseInsensitiveForKeys()
        {
            List<XamlComponentCustomProperty> properties = new List<XamlComponentCustomProperty>();

            properties.AddCustomProperty("Grid.Column", "1");
            properties.AddCustomProperty("grid.column", "2");

            Assert.Single(properties);
            Assert.True(properties.IsPropertyExistsWithValue("GRID.COLUMN", "1"));
        }

        [Fact]
        public void ToPropertyMap_CreatesCaseInsensitiveLookup()
        {
            List<XamlComponentCustomProperty> properties = new List<XamlComponentCustomProperty>
            {
                new XamlComponentCustomProperty("Grid.Column", "2")
            };

            Dictionary<string, string> map = properties.ToPropertyMap();

            Assert.True(map.ContainsKey("grid.column"));
            Assert.Equal("2", map["GRID.COLUMN"]);
        }
    }
}
