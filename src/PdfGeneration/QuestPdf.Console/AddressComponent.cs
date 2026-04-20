using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPdf.Console;

public class AddressComponent(string title, Address address) : IComponent
{
    public void Compose(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(2);

            column.Item().BorderBottom(1).PaddingBottom(5).Text(title).SemiBold();

            column.Item().Text(address.CompanyName);
            column.Item().Text(address.Street);
            column.Item().Text($"{address.City}, {address.State}");
            column.Item().Text(address.Email);
            column.Item().Text(address.Phone);
        });
    }
}
