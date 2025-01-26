namespace StateMachines.Stateless.ExampleAPI.PhoneCall;

public class PhoneCallSMV2
{
    private readonly StateMachine<PhoneState, PhoneTrigger> _phoneCall;
    public PhoneState State => _phoneCall.State;
    public StateMachine<PhoneState, PhoneTrigger> StateMachine => _phoneCall;

    public PhoneCallSMV2()
    {
        _phoneCall = new StateMachine<PhoneState, PhoneTrigger>(PhoneState.OnHook);

        _phoneCall.Configure(PhoneState.OnHook)
            .Permit(PhoneTrigger.LiftPhone, PhoneState.OffHook);
            
        _phoneCall.Configure(PhoneState.OffHook)
            .Permit(PhoneTrigger.CallDialled, PhoneState.Ringing)
            .Permit(PhoneTrigger.SlammedPhone, PhoneState.OnHook);
            
        _phoneCall.Configure(PhoneState.Ringing)
            .Permit(PhoneTrigger.SlammedPhone, PhoneState.OnHook)
            .Permit(PhoneTrigger.CallConnected, PhoneState.Connected);  
            
        _phoneCall.Configure(PhoneState.OnHold)
            .Permit(PhoneTrigger.SlammedPhone, PhoneState.OnHook)
            .Permit(PhoneTrigger.RemovedFromHold, PhoneState.Connected);

        _phoneCall.Configure(PhoneState.Connected)
            // .OnEntry(t => StartCallTimer())
            // .OnExit(t => StopCallTimer())
            // .InternalTransition(PhoneTrigger.MuteMicrophone, t => OnMute())
            // .InternalTransition(PhoneTrigger.UnmuteMicrophone, t => OnUnmute())
            // .InternalTransition<int>(_setVolumeTrigger, (volume, t) => OnSetVolume(volume))
            .Permit(PhoneTrigger.SlammedPhone, PhoneState.OnHook)
            .Permit(PhoneTrigger.LeftMessage, PhoneState.OffHook)
            .Permit(PhoneTrigger.PlacedOnHold, PhoneState.OnHold);
    }

    public void Fire(PhoneTrigger trigger)
    {
        _phoneCall.Fire(trigger);
    }


    private void OnUnmute()
    {
    }

    private void OnMute()
    {
    }

    private void StopCallTimer()
    {
    }

    private void StartCallTimer()
    {
    }
}