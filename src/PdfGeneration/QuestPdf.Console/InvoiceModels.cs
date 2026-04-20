namespace QuestPdf.Console;

public class InvoiceModel
{
    public int InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }

    public required Address SellerAddress { get; set; }
    public required Address CustomerAddress { get; set; }

    public required List<OrderItem> Items { get; set; }
    public string? Comments { get; set; }
}

public class OrderItem
{
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class Address
{
    public required string CompanyName { get; set; }
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
}
