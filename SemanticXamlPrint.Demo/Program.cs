using Newtonsoft.Json;
using SemanticXamlPrint.Components;
using System;

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
                DefaultXamlParser parser = new DefaultXamlParser();
                IXamlComponent rootObject = parser.Parse("receipt.restaurant.withLogo.template");
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
