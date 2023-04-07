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
                //Get Template Contents
                byte[] xamlFileBytes = File.ReadAllBytes("custom.grid.template");

                //Use Default Parser
                DefaultXamlParser parser = new DefaultXamlParser();
                IXamlComponent rootObject = parser.Parse(xamlFileBytes);

                //Use Print Service to Print 
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
