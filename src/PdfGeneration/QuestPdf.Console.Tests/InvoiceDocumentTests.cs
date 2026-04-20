namespace QuestPdf.Console.Tests;

public class InvoiceDocumentTests
{
    [Test]
    public Task Invoice_document_matches_snapshot()
    {
        var document = new InvoiceDocument(MockInvoiceModel());

        return Verify(document);
    }

    private static InvoiceModel MockInvoiceModel()
    {
        var seller = new Address
        {
            CompanyName = "Acme Supply Co.",
            Street = "100 Industrial Way",
            City = "Springfield",
            State = "IL",
            Email = "billing@acme.example",
            Phone = "+1 555-0100"
        };

        var customer = new Address
        {
            CompanyName = "Contoso Ltd.",
            Street = "200 Commerce Blvd",
            City = "Columbus",
            State = "OH",
            Email = "ap@contoso.example",
            Phone = "+1 555-0200"
        };

        return new InvoiceModel
        {
            InvoiceNumber = 4242,
            IssueDate = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            DueDate = new DateTime(2026, 1, 29, 0, 0, 0, DateTimeKind.Utc),
            SellerAddress = seller,
            CustomerAddress = customer,
            Items =
            [
                new OrderItem { Name = "Widget A", Price = 10.50m, Quantity = 2 },
                new OrderItem { Name = "Widget B", Price = 25.00m, Quantity = 1 },
                new OrderItem { Name = "Service hours", Price = 150.00m, Quantity = 3 }
            ],
            Comments = "Thank you for your business."
        };
    }
}