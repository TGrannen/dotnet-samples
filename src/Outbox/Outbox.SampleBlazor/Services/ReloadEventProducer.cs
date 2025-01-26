namespace Outbox.SampleBlazor.Services;

public class ReloadEventProducer
{
    public EventHandler<EventArgs> MessageProcessed; // event


    public void SendReload()
    {
        MessageProcessed?.Invoke(this, EventArgs.Empty);
    }
}