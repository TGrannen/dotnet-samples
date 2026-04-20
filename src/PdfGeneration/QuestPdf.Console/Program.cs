using QuestPDF.Companion;
using QuestPdf.Console;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

// https://www.questpdf.com/license/configuration
QuestPDF.Settings.License = LicenseType.Community;

var model = InvoiceDocumentDataSource.GetInvoiceDetails();
var document = new InvoiceDocument(model);

var outputPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "invoice.pdf"));
document.GeneratePdf(outputPath);

document.ShowInCompanion();

Console.WriteLine($"Wrote {outputPath}");