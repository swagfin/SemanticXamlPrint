# SemanticXamlPrint

SemanticXamlPrint helps you build printable documents (receipts, invoices, reports) using a simple XML/XAML-like template.

Instead of hardcoding drawing coordinates, you define your layout once and render it to:
- `System.Drawing` print output
- PDF (`PdfSharp`)
- PDF for .NET Core (`PdfSharpCore`)

## Why Use It?

- Fast template-driven document design
- Great for thermal receipts and A4 layouts
- Shared structure across Print and PDF outputs
- Supports nested grids, text wrapping, images, QR codes, borders, and line breaks
- Supports fixed-height or page-filling invoice item grids (A4-friendly)

## Install

NuGet:

```powershell
Install-Package SemanticXamlPrint
```

Package:
https://nuget.org/packages/SemanticXamlPrint

## Quick Integration Guide

### 1. Create a template file

Example `invoice.template`:

```xaml
<Template Font="Calibri" FontSize="10" Document="A4" MarginTop="20" MarginBottom="20" MarginLeft="20" MarginRight="20">

  <Grid ColumnWidths="6*1.5*1.5*1.5" BorderStyle="Solid" HeightMode="FillRemaining" BottomReserve="120">
    <GridRow>
      <Data Grid.Column="0" FontStyle="Bold" Align="Center">DESCRIPTION</Data>
      <Data Grid.Column="1" FontStyle="Bold" Align="Center">HOURS</Data>
      <Data Grid.Column="2" FontStyle="Bold" Align="Center">RATE</Data>
      <Data Grid.Column="3" FontStyle="Bold" Align="Center">AMOUNT</Data>
    </GridRow>

    <GridRow>
      <Data Grid.Column="0">Test Example</Data>
      <Data Grid.Column="1" Align="Center">2</Data>
      <Data Grid.Column="2" Align="Right">1,200</Data>
      <Data Grid.Column="3" Align="Right">2,400</Data>
    </GridRow>
  </Grid>

</Template>
```

### 2. Parse the template

```csharp
byte[] xamlFileBytes = File.ReadAllBytes("invoice.template");
IXamlComponent xamlComponent = DefaultXamlParser.Parse(xamlFileBytes);
```

### 3. Render to your target

PrintDocument (`System.Drawing`):

```csharp
PrintDocument printDocument = new PrintDocument();
printDocument.PrintPage += (sender, eventArgs) =>
{
    eventArgs.DrawXamlComponent(xamlComponent);
};
printDocument.Print();
```

PDF (`SemanticXamlPrint.PDF`):

```csharp
using (PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument())
{
    document.DrawXamlComponent(xamlComponent);
    document.Save("output.pdf");
}
```

PDF .NET Core (`SemanticXamlPrint.PDF.NetCore`):

```csharp
using (PdfSharpCore.Pdf.PdfDocument document = new PdfSharpCore.Pdf.PdfDocument())
{
    document.DrawXamlComponent(xamlComponent);
    document.Save("outputcore.pdf");
}
```

## A4 Invoice Grid Stretch (Fill Remaining Page)

For invoice designs where the items table should occupy the remaining page height:

```xaml
<Grid ColumnWidths="6*1.5*1.5*1.5" BorderStyle="Solid" HeightMode="FillRemaining" BottomReserve="120">
  <!-- header + item rows -->
</Grid>
```

Notes:
- `HeightMode="FillRemaining"`: expands the grid down to page bottom
- `BottomReserve="120"`: keeps free space for totals/footer/signature
- `MinHeight="300"`: optional minimum grid height fallback

## Usage Example (Basic Receipt)

```csharp
class Program
{
    static void Main(string[] args)
    {
        byte[] xamlFileBytes = File.ReadAllBytes("custom.grid.template");
        IXamlComponent xamlComponent = DefaultXamlParser.Parse(xamlFileBytes);

        PrintDocument printDocument = new PrintDocument();
        printDocument.PrintPage += (obj, eventArgs) =>
        {
            eventArgs.DrawXamlComponent(xamlComponent);
        };
        printDocument.Print();
    }
}
```

## Result

![Result](https://github.com/swagfin/SemanticXamlPrint/blob/a3a0b443bc8e1c7d3eb9ee6b9e9a92643a14901d/Screenshots/sample-grid.jpg)

## Supported Template Components

### 1. Template

```xaml
<Template Font="Calibri" FontSize="10" MarginTop="10">
   <!-- Components -->
</Template>
```

### 2. Data

```xaml
<Data FontStyle="Bold" FontSize="11" TextWrap="True" Align="Center">Centered wrapped text</Data>
```

### 3. Image

```xaml
<Image Source="logo.png" Width="100" Height="100" />
```

### 4. Grid + GridRow

```xaml
<Grid ColumnWidths="1*4*2" BorderStyle="Solid">
  <GridRow>
    <Data Grid.Column="0" FontStyle="Bold">Column 1</Data>
    <Data Grid.Column="1" FontStyle="Bold">Column 2</Data>
    <Data Grid.Column="2" FontStyle="Bold" Align="Right">Column 3</Data>
  </GridRow>
</Grid>
```

### 5. Cells + Cell

```xaml
<Cells>
  <Cell FontStyle="Bold" X="0">ITEM DESC.</Cell>
  <Cell FontStyle="Bold" X="120">RATE</Cell>
  <Cell FontStyle="Bold" X="170">QTY</Cell>
  <Cell FontStyle="Bold" X="220">AMOUNT</Cell>
</Cells>
```

### 6. QRCode

```xaml
<QRCode Text="https://example.com/invoice/1001" Width="100" Height="100" />
```

### 7. Line

```xaml
<Line Style="Dash" />
```

### 8. LineBreak

```xaml
<LineBreak />
```

## More Examples

Demo projects:
- https://github.com/swagfin/SemanticXamlPrint/tree/master/SemanticXamlPrint.Demo
- https://github.com/swagfin/SemanticXamlPrint/tree/master/SemanticXamlPrint.DemoNetCore

## Starter Templates

Use these as quick copy/paste starting points.

### 1. Receipt (Thermal Style)

```xaml
<Template Font="Calibri" FontSize="10" MarginTop="10" MarginLeft="5" MarginRight="5">
  <Data Align="Center" FontStyle="Bold" FontSize="12">MY STORE</Data>
  <Data Align="Center">123 Main Street</Data>
  <LineBreak />
  <Line />
  <Grid ColumnWidths="1*4*2">
    <GridRow>
      <Data Grid.Column="0" FontStyle="Bold">QTY</Data>
      <Data Grid.Column="1" FontStyle="Bold">ITEM</Data>
      <Data Grid.Column="2" FontStyle="Bold" Align="Right">AMOUNT</Data>
    </GridRow>
    <GridRow>
      <Data Grid.Column="0">1</Data>
      <Data Grid.Column="1" TextWrap="True">Coffee</Data>
      <Data Grid.Column="2" Align="Right">3.50</Data>
    </GridRow>
  </Grid>
  <Line />
  <Data Align="Right" FontStyle="Bold">TOTAL: 3.50</Data>
  <LineBreak />
  <Data Align="Center">Thank you!</Data>
</Template>
```

### 2. Invoice (A4, Fill Remaining Items Grid)

```xaml
<Template Font="Calibri" FontSize="10" Document="A4" MarginTop="20" MarginBottom="20" MarginLeft="20" MarginRight="20">
  <Grid ColumnWidths="1*1">
    <GridRow>
      <Data Grid.Column="0" FontStyle="Bold" FontSize="16">[Your Company Name]</Data>
      <Data Grid.Column="1" Align="Right" FontStyle="Bold" FontSize="18">INVOICE</Data>
    </GridRow>
  </Grid>
  <LineBreak />
  <Grid ColumnWidths="6*1.5*1.5*1.5" BorderStyle="Solid" HeightMode="FillRemaining" BottomReserve="120">
    <GridRow>
      <Data Grid.Column="0" FontStyle="Bold" Align="Center">DESCRIPTION</Data>
      <Data Grid.Column="1" FontStyle="Bold" Align="Center">HOURS</Data>
      <Data Grid.Column="2" FontStyle="Bold" Align="Center">RATE</Data>
      <Data Grid.Column="3" FontStyle="Bold" Align="Center">AMOUNT</Data>
    </GridRow>
    <GridRow>
      <Data Grid.Column="0">Test Example</Data>
      <Data Grid.Column="1" Align="Center">2</Data>
      <Data Grid.Column="2" Align="Right">1,200</Data>
      <Data Grid.Column="3" Align="Right">2,400</Data>
    </GridRow>
  </Grid>
  <Data Align="Right" FontStyle="Bold">TOTAL: 2,400</Data>
</Template>
```

### 3. Report (Section + Summary)

```xaml
<Template Font="Calibri" FontSize="10" Document="A4" MarginTop="20" MarginBottom="20" MarginLeft="20" MarginRight="20">
  <Data FontStyle="Bold" FontSize="14">Monthly Sales Report</Data>
  <Data>Period: January 2026</Data>
  <LineBreak />
  <Grid ColumnWidths="3*2*2" BorderStyle="Solid">
    <GridRow>
      <Data Grid.Column="0" FontStyle="Bold">Category</Data>
      <Data Grid.Column="1" FontStyle="Bold" Align="Right">Orders</Data>
      <Data Grid.Column="2" FontStyle="Bold" Align="Right">Revenue</Data>
    </GridRow>
    <GridRow>
      <Data Grid.Column="0">Beverages</Data>
      <Data Grid.Column="1" Align="Right">120</Data>
      <Data Grid.Column="2" Align="Right">4,800.00</Data>
    </GridRow>
  </Grid>
  <LineBreak />
  <Data Align="Right" FontStyle="Bold">Grand Total: 4,800.00</Data>
</Template>
```
