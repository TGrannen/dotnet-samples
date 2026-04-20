using QuestPDF.Companion;
using QuestPdf.Console;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

// https://www.questpdf.com/license/configuration
QuestPDF.Settings.License = LicenseType.Community;

var model = InvoiceDocumentDataSource.GetInvoiceDetails();
var invoice = new InvoiceDocument(model);
var showcase = new GraphicsShowcaseDocument();

var invoicePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "invoice.pdf"));
var showcasePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "graphics-showcase.pdf"));

invoice.GeneratePdf(invoicePath);
showcase.GeneratePdf(showcasePath);

// invoice.ShowInCompanion();
showcase.ShowInCompanion();

Console.WriteLine($"Wrote {invoicePath}");
Console.WriteLine($"Wrote {showcasePath}");