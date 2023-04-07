using Newtonsoft.Json;
using SemanticXamlPrint.Components;
using System;
using System.IO;

namespace SemanticXamlPrint.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TESTING XAML PRINT");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            try
            {
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "custom.grid.template");
                if (!File.Exists(templatePath)) throw new Exception($"template could not be found: {templatePath}");
                byte[] xamlFileBytes = File.ReadAllBytes(templatePath);
                //Proceed
                DefaultXamlParser parser = new DefaultXamlParser();
                IXamlComponent rootObject = parser.Parse(xamlFileBytes);
                //We have Xaml Objects
                string jsonResponse = JsonConvert.SerializeObject(rootObject, Formatting.Indented);
                Console.WriteLine(jsonResponse);

                DefaultPrintService printService = new DefaultPrintService();
                printService.Print(rootObject, "POS-80");
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message.ToString());
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.ReadLine();
        }
    }
}
