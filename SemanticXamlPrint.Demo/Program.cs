using Newtonsoft.Json;
using SemanticXamlPrint.Components;
using SemanticXamlPrint.Parsers;
using System;

namespace SemanticXamlPrint.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TESTING XAML PRINT");

            DefaultXamlParser parser = new DefaultXamlParser();
            IXamlComponent rootObject = parser.Parse("custom.receipt.template");

            string jsonResponse = JsonConvert.SerializeObject(rootObject, Formatting.Indented);

            Console.WriteLine(jsonResponse);


            DefaultPrintService printService = new DefaultPrintService();
            printService.Print(rootObject, "POS-80");
            Console.ReadLine();
        }
    }
}
