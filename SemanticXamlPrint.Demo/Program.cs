using SemanticXamlPrint.Components;
using System;
using System.Drawing.Printing;
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
                IXamlComponent xamlComponent = DefaultXamlParser.Parse(xamlFileBytes);

                PrintDocument printDocument = new PrintDocument();
                printDocument.PrintPage += (obj, eventAgs) =>
                {
                    //Use Xaml Draw Extension to Print
                    eventAgs.Graphics.DrawXamlComponent(xamlComponent);
                };
                printDocument.PrinterSettings.PrinterName = "POS-80";
                printDocument.Print();

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
