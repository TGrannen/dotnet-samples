namespace StateMachines.Stateless.ExampleAPI.PhoneCall;

public enum PhoneTrigger
{
        
    CallDialled,
    MuteMicrophone,
    UnmuteMicrophone,
    LeftMessage,
    LiftPhone,
    SlammedPhone,
    PlacedOnHold,
    CallConnected,
    RemovedFromHold
}