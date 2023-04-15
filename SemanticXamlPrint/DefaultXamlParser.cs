using SemanticXamlPrint.Components;
using System.IO;
using System.Xml;

namespace SemanticXamlPrint
{
    public static class DefaultXamlParser
    {
        /// <summary>
        /// SemanticXamlPrint Extension Method Pass File Bytes to an IXamlComponent
        /// </summary>
        /// <param name="xamlFileBytes"></param>
        /// <returns></returns>
        public static IXamlComponent Parse(this byte[] xamlFileBytes)
        {
            using (MemoryStream stream = new MemoryStream(xamlFileBytes))
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(stream);
                var rootNode = xmlDocument.DocumentElement;
                return rootNode.CreateComponentFromXml();
            }
        }
    }
}
