namespace StateMachines.Stateless.ExampleAPI.DynamicSM;

public class StateSetup
{
    public State CurrentState { get; set; }
    public Trigger Trigger { get; set; }
    public State DestinationState { get; set; }
}