# SemanticXamlPrint

Simple Library to help you with creating receipts using simple Xaml like syntax. This library uses System.Drawing to generate and print receipts

## Overview

* Generate a receipt i.e. Thermal 80mm receipt or A4 receipt
* Uses Xaml like Syntax to generate a receipt template
* Note, this library targets .Net Framework 4.5

## Install 

*NuGet Package*
```
Install-Package SemanticXamlPrint
```
https://nuget.org/packages/SemanticXamlPrint

## Usage Example

```cs
    class Program
    {
        static void Main(string[] args)
        {

            //Get Template Contents
            byte[] xamlFileBytes = File.ReadAllBytes("custom.grid.template");
            
            //Use Default Parser
            DefaultXamlParser parser = new DefaultXamlParser();
            IXamlComponent rootObject = parser.Parse(xamlFileBytes);
            
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += (obj, eventAgs) =>
            {
                //Use Xaml Draw Extension to Print
                eventAgs.Graphics.DrawXamlComponent(rootObject);
            };
            printDocument.PrinterSettings.PrinterName = "POS-80";
            printDocument.Print();

            Console.ReadLine();
            Environment.Exit(0);
        }
    }
```

## Results

![Result](https://github.com/swagfin/SemanticXamlPrint/blob/a3a0b443bc8e1c7d3eb9ee6b9e9a92643a14901d/Screenshots/sample-grid.jpg)


## File custom.grid.template
```xaml
<Template font="Calibri" FontSize="10" MaxWidth="290" MarginTop="10">

	<Grid ColumnWidths="1*4*2" BorderStyle="Solid">
		<GridRow>
			<Data Grid.Column="0" FontStyle="Bold" Align="Center">QTY</Data>
			<Data Grid.Column="1" FontStyle="Bold">ITEM DESC.</Data>
			<Data Grid.Column="2" FontStyle="Bold" Align="Right">AMOUNT</Data>
		</GridRow>
	</Grid>

	<Grid ColumnWidths="1*4*2">
		<GridRow>
			<Data Grid.Column="0" Align="Center">1</Data>
			<Data Grid.Column="1" TextWrap ="True">Chips with Vegetable Salad at a Discounted Price</Data>
			<Data Grid.Column="2" Align="Right">250.00</Data>
		</GridRow>

		<GridRow>
			<Data Grid.Column="0" Align="Center">1</Data>
			<Data Grid.Column="1" TextWrap ="True">HEINKEN</Data>
			<Data Grid.Column="2" Align="Right">300.00</Data>
		</GridRow>
		<GridRow>
			<Data Grid.Column="0" Align="Center">1</Data>
			<Data Grid.Column="1" TextWrap ="True">PILSNER</Data>
			<Data Grid.Column="2" Align="Right">250.00</Data>
		</GridRow>
	</Grid>
</Template>
```

## Check out more examples on the Demo Project
[SemanticXamlPrint.Demo](https://github.com/swagfin/SemanticXamlPrint/tree/master/SemanticXamlPrint.Demo)