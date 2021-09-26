using Stateless;

namespace StateMachines.Stateless.ExampleAPI.PhoneCall
{
    public class PhoneCallSm
    {
        private readonly StateMachine<PhoneState, PhoneTrigger> _phoneCall;
        public PhoneState State => _phoneCall.State;
        public StateMachine<PhoneState, PhoneTrigger> StateMachine => _phoneCall;

        public PhoneCallSm()
        {
            _phoneCall = new StateMachine<PhoneState, PhoneTrigger>(PhoneState.OffHook);

            _phoneCall.Configure(PhoneState.OffHook)
                .Permit(PhoneTrigger.CallDialled, PhoneState.Ringing);

            _phoneCall.Configure(PhoneState.Connected)
                .OnEntry(t => StartCallTimer())
                .OnExit(t => StopCallTimer())
                .InternalTransition(PhoneTrigger.MuteMicrophone, t => OnMute())
                .InternalTransition(PhoneTrigger.UnmuteMicrophone, t => OnUnmute())
                // .InternalTransition<int>(_setVolumeTrigger, (volume, t) => OnSetVolume(volume))
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
}