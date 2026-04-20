# Pdf Generation

This area demonstrates PDF generation with [QuestPDF](https://www.questpdf.com/) using a small console sample based on the [In-Depth Invoice Tutorial](https://www.questpdf.com/invoice-tutorial.html).

## Projects

| Project | Description |
| --- | --- |
| [QuestPdf.Console](QuestPdf.Console/) | Console app: loads sample invoice data, renders an `IDocument`, writes `invoice.pdf` next to the build output. |
| [QuestPdf.Console.Tests](QuestPdf.Console.Tests/) | [TUnit](https://tunit.dev/) tests that snapshot the rendered document with [Verify.QuestPDF](https://github.com/VerifyTests/Verify.QuestPDF) and [Verify.ImageMagick](https://github.com/VerifyTests/Verify.ImageMagick) for tolerant image comparison. |

Solution file: [PdfGeneration.sln](PdfGeneration.sln).

## Run the console app

From the repository root:

```powershell
dotnet run --project src/PdfGeneration/QuestPdf.Console/QuestPdf.Console.csproj
```

Open `invoice.pdf` under `src/PdfGeneration/QuestPdf.Console/bin/Debug/net10.0/` or `.../Release/net10.0/` for the configuration you used.

The app uses a **Community** QuestPDF license in code; adjust if your scenario requires a different tier ([license configuration](https://www.questpdf.com/license/configuration)).

## Layout (invoice sample)

The invoice example follows QuestPDF’s suggested split:

1. **Models** — `InvoiceModel`, `OrderItem`, `Address` in `InvoiceModels.cs`.
2. **Data source** — `InvoiceDocumentDataSource`: `GetInvoiceDetails()` uses random placeholder data for local demos; `GetDeterministicInvoiceDetails()` is fixed data for tests and stable snapshots.
3. **Template** — `InvoiceDocument` (`IDocument`) and reusable `AddressComponent` (`IComponent`).

## Tests (TUnit + Verify)

Tests call `Verify(document)` on a QuestPDF `IDocument`. Verify serializes metadata (for example page count and document settings) and compares each page as a PNG. [Verify.ImageMagick](https://github.com/VerifyTests/Verify.ImageMagick) registers comparers so minor rendering differences across machines are less likely to fail the build ([Verify.QuestPDF readme](https://github.com/VerifyTests/Verify.QuestPDF)).

`ModuleInitializer` configures QuestPDF license, `VerifyQuestPdf.Initialize()`, and ImageMagick comparers.

Run tests **from this folder** so `global.json` picks the Microsoft Testing Platform runner (needed for `dotnet test` on .NET 10 with TUnit’s MTP integration):

```powershell
Set-Location src/PdfGeneration
dotnet test QuestPdf.Console.Tests/QuestPdf.Console.Tests.csproj
```

Alternatively you can run the test project executable directly:

```powershell
dotnet run --project src/PdfGeneration/QuestPdf.Console.Tests/QuestPdf.Console.Tests.csproj -c Release
```

### Updating snapshots

When you intentionally change layout or content, Verify will write `*.received.*` next to the test source. Review the diff, then replace the corresponding `*.verified.*` files (or use your Verify workflow / diff tool accept flow). Received files are ignored by Git via [`.gitignore`](.gitignore).

## Companion app (optional)

For live preview and hot reload while editing layouts, see the [QuestPDF Companion](https://www.questpdf.com/companion/usage.html) (`ShowInCompanion()` from the QuestPDF companion integration). A JetBrains Rider run configuration for `dotnet watch` on the console app lives under [`.run`](.run/).

## Further reading

* [QuestPDF quick start](https://www.questpdf.com/quick-start.html)
* [Invoice tutorial](https://www.questpdf.com/invoice-tutorial.html)
* [Verify.QuestPDF](https://github.com/VerifyTests/Verify.QuestPDF)
