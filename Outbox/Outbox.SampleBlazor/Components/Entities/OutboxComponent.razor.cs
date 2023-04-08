using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Components;
using Nefarius.Blazor.EventAggregator;
using Outbox.DynamoDb.Internal;

namespace Outbox.SampleBlazor.Components.Entities;

public partial class OutboxComponent : ComponentBase, IHandle<ReloadEvent>
{
    private IEnumerable<OutboxMessage> _elements = new List<OutboxMessage>();
    [Inject] private IEventAggregator EventAggregator { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        EventAggregator.Subscribe(this);
    }

    public async Task HandleAsync(ReloadEvent message)
    {
        await InvokeAsync(async () =>
        {
            await Reload();
            StateHasChanged();
        });
    }

    private async Task Reload()
    {
        _elements = new List<OutboxMessage>();
        _elements = await DynamoDbContext.ScanAsync<OutboxMessage>(new List<ScanCondition>()).GetRemainingAsync();
    }
}