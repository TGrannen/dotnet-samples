using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ScottPlot;
using ZXing;
using ZXing.OneD;
using ZXing.QrCode;
using ZXing.Rendering;
using QColors = QuestPDF.Helpers.Colors;

namespace QuestPdf.Console;

/// <summary>
/// Demonstrates QuestPDF patterns from the docs for SVG, barcodes (ZXing → SVG), and charts (ScottPlot → SVG):
/// <see href="https://www.questpdf.com/api-reference/image/svg.html">SVG</see>,
/// <see href="https://www.questpdf.com/api-reference/barcodes.html">Barcodes</see>,
/// <see href="https://www.questpdf.com/api-reference/charts.html">Charts</see>.
/// </summary>
public sealed class GraphicsShowcaseDocument : IDocument
{
    private const string DemoQrPayload = "https://www.questpdf.com/";

    /// <summary>Valid EAN-8 (7 digits + checksum); fixed for reproducible output.</summary>
    private const string DemoEan8 = "12345670";

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(40);
            page.PageColor(QColors.White);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Content().Column(column =>
            {
                column.Spacing(16);

                column.Item().Text("QuestPDF graphics showcase").FontSize(18).SemiBold();
                column.Item().Text(
                    "Static SVG, ZXing barcodes rendered to SVG, and ScottPlot charts exported as SVG (QuestPDF .Svg(...)).");

                column.Item().Element(ComposeStaticSvgSection);
                column.Item().Element(ComposeBarcodeSection);
            });
        });

        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(40);
            page.PageColor(QColors.White);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Content().Element(ComposeChartsSection);
        });
    }

    private static void ComposeStaticSvgSection(IContainer container)
    {
        container.Border(1).BorderColor(QColors.Grey.Lighten2).Padding(15).Column(section =>
        {
            section.Spacing(8);
            section.Item().Text("Inline SVG").FontSize(12).SemiBold();
            section.Item().Text("Simple vector icon embedded as a string (see SVG Support docs).");

            section.Item().Width(120).Svg(SimpleVectorIconSvg).FitArea();
        });
    }

    /// <summary>Minimal inline SVG (no external file).</summary>
    private const string SimpleVectorIconSvg =
        """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100">
          <defs>
            <linearGradient id="g" x1="0%" y1="0%" x2="100%" y2="100%">
              <stop offset="0%" style="stop-color:#2563ed;stop-opacity:1" />
              <stop offset="100%" style="stop-color:#7c3aeb;stop-opacity:1" />
            </linearGradient>
          </defs>
          <rect x="8" y="8" width="84" height="84" rx="18" fill="url(#g)"/>
          <path d="M30 52 L45 67 L72 38" fill="none" stroke="white" stroke-width="8" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
        """;

    private void ComposeBarcodeSection(IContainer container)
    {
        container.Border(1).BorderColor(QColors.Grey.Lighten2).Padding(15).Column(section =>
        {
            section.Spacing(12);
            section.Item().Text("Barcodes via ZXing.Net (SVG renderer)").FontSize(12).SemiBold();

            section.Item().Background(QColors.Grey.Lighten3).Padding(20).Row(row =>
            {
                row.Spacing(20);

                row.RelativeItem().Column(textCol =>
                {
                    textCol.Spacing(6);
                    textCol.Item().Text(t =>
                    {
                        t.Span("EAN-8 product code: ").SemiBold();
                        t.Span(DemoEan8);
                    });
                    textCol.Item().Text("QR encodes the QuestPDF site URL (same pattern as the barcodes doc).");
                });

                row.AutoItem()
                    .Background(QColors.White)
                    .AlignCenter()
                    .AlignMiddle()
                    .Width(200)
                    .Height(75)
                    .Svg(size =>
                    {
                        var writer = new EAN8Writer();
                        var matrix = writer.encode(DemoEan8, BarcodeFormat.EAN_8, (int)size.Width, (int)size.Height);
                        var renderer = new SvgRenderer { FontName = "Lato", FontSize = 14 };
                        return renderer.Render(matrix, BarcodeFormat.EAN_8, DemoEan8).Content;
                    });

                row.ConstantItem(5, Unit.Centimetre)
                    .AspectRatio(1)
                    .Background(QColors.White)
                    .Svg(size =>
                    {
                        var writer = new QRCodeWriter();
                        var matrix = writer.encode(DemoQrPayload, BarcodeFormat.QR_CODE, (int)size.Width, (int)size.Height);
                        var renderer = new SvgRenderer { FontName = "Lato" };
                        // Use QR_CODE for rendering (QuestPDF’s QR sample mistakenly used EAN_13 here).
                        return renderer.Render(matrix, BarcodeFormat.QR_CODE, DemoQrPayload).Content;
                    });
            });
        });
    }

    private static void ComposeChartsSection(IContainer container)
    {
        container.Border(1).BorderColor(QColors.Grey.Lighten2).Padding(15).Column(section =>
        {
            section.Spacing(14);
            section.Item().Text("Charts via ScottPlot (GetSvgHtml)").FontSize(12).SemiBold();

            section.Item().Column(chartCol =>
            {
                chartCol.Spacing(10);
                chartCol.Item().Text("US energy consumption [%] by source in 2021 (pie from charts doc, ScottPlot 5 API).")
                    .AlignCenter().SemiBold();

                chartCol.Item()
                    .Height(220)
                    .Svg(size =>
                    {
                        Plot plot = new();

                        List<PieSlice> slices =
                        [
                            new() { Value = 8, FillColor = new ScottPlot.Color(QColors.Yellow.Medium.Hex), Label = "Nuclear" },
                            new() { Value = 12, FillColor = new ScottPlot.Color(QColors.Green.Medium.Hex), Label = "Renewable" },
                            new() { Value = 32, FillColor = new ScottPlot.Color(QColors.Blue.Medium.Hex), Label = "Natural gas" },
                            new() { Value = 11, FillColor = new ScottPlot.Color(QColors.Grey.Medium.Hex), Label = "Coal" },
                            new() { Value = 36, FillColor = new ScottPlot.Color(QColors.Brown.Medium.Hex), Label = "Petroleum" }
                        ];

                        var pie = plot.Add.Pie(slices);
                        pie.DonutFraction = 0.5;
                        pie.SliceLabelDistance = 1.5;
                        pie.LineColor = ScottPlot.Colors.White;
                        pie.LineWidth = 3;

                        foreach (var slice in pie.Slices)
                        {
                            slice.LabelStyle.FontName = "Lato";
                            slice.LabelStyle.FontSize = 14;
                        }

                        plot.Axes.Frameless();
                        plot.HideGrid();

                        return plot.GetSvgHtml((int)size.Width, (int)size.Height);
                    });
            });

            section.Item().PaddingTop(8).Column(chartCol =>
            {
                chartCol.Spacing(10);
                chartCol.Item().Text("Popularity of C# versions in 2023 (bar chart from charts doc).")
                    .AlignCenter().SemiBold();

                chartCol.Item()
                    .Height(200)
                    .Svg(size =>
                    {
                        Plot plot = new();

                        Bar[] bars =
                        [
                            new() { Position = 1, Value = 2 },
                            new() { Position = 2, Value = 3 },
                            new() { Position = 3, Value = 8 },
                            new() { Position = 4, Value = 13 },
                            new() { Position = 5, Value = 17 },
                            new() { Position = 6, Value = 17 },
                            new() { Position = 7, Value = 32 },
                            new() { Position = 8, Value = 42 }
                        ];

                        foreach (var bar in bars)
                        {
                            bar.FillColor = new ScottPlot.Color(QColors.Grey.Medium.Hex);
                            bar.LineWidth = 0;
                            bar.Size = 0.5;
                        }

                        plot.Add.Bars(bars);

                        Tick[] ticks =
                        [
                            new(1, "Other"),
                            new(2, "C# 5"),
                            new(3, "C# 6"),
                            new(4, "C# 7"),
                            new(5, "C# 8"),
                            new(6, "C# 9"),
                            new(7, "C# 10"),
                            new(8, "C# 11")
                        ];

                        plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
                        plot.Axes.Bottom.MajorTickStyle.Length = 0;
                        plot.Axes.Bottom.TickLabelStyle.FontName = "Lato";
                        plot.Axes.Bottom.TickLabelStyle.FontSize = 12;
                        plot.Axes.Bottom.TickLabelStyle.OffsetY = 6;
                        plot.Grid.XAxisStyle.IsVisible = false;
                        plot.Axes.Margins(bottom: 0, top: 0.25f);

                        return plot.GetSvgHtml((int)size.Width, (int)size.Height);
                    });
            });
        });
    }
}