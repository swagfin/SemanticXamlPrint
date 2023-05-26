using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SemanticXamlPrint.Parser;
using SemanticXamlPrint.Parser.Components;
using SemanticXamlPrint.PDF.NetCore;
using System;
using System.IO;
namespace SemanticXamlPrint.DemoNetCore
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
                //####  PDF SHARP #####
                using (PdfDocument document = new PdfDocument())
                {
                    // Add a page to the document
                    PdfPage page = document.AddPage();
                    // Create a graphics object for the page
                    XGraphics xgraphics = XGraphics.FromPdfPage(page);
                    //Use Xaml Draw Extension to Generate PDF
                    xgraphics.DrawXamlComponent(xamlComponent);
                    // Save the PDF document to a file
                    document.Save("outputcore.pdf");
                }
                //####  PDF SHARP #####
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
