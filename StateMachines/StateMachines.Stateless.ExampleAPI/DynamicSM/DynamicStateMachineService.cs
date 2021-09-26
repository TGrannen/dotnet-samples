using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stateless;

namespace StateMachines.Stateless.ExampleAPI.DynamicSM
{
    public class DynamicStateMachineService
    {
        private readonly ILogger<DynamicStateMachineService> _logger;

        public DynamicStateMachineService(ILogger<DynamicStateMachineService> logger)
        {
            _logger = logger;
        }

        public IEnumerable<StateSetup> ConvertFromStrings(IEnumerable<string> strings)
        {
            foreach (string s in strings)
            {
                var split = s.Split(",").Select(x => x.Trim()).ToList();
                yield return new StateSetup
                {
                    CurrentState = (State) Enum.Parse(typeof(State), split[0]),
                    Trigger = (Trigger) Enum.Parse(typeof(Trigger), split[1]),
                    DestinationState = (State) Enum.Parse(typeof(State), split[2]),
                };
            }
        }

        public StateMachine<State, Trigger> CreateStateMachine(IEnumerable<StateSetup> setups, State initialState)
        {
            var machine = new StateMachine<State, Trigger>(initialState);

            var list = setups.ToList();
            foreach (var grp in list.GroupBy(x => x.CurrentState))
            {
                var test = machine.Configure(grp.Key);
                foreach (var setup in grp)
                {
                    if (setup.CurrentState == setup.DestinationState)
                    {
                        _logger.LogWarning("State Reentry. Setup {@Setup}", setup);
                        test.PermitReentry(setup.Trigger);
                    }
                    else
                    {
                        test.Permit(setup.Trigger, setup.DestinationState);
                    }
                }
            }

            return machine;
        }
    }
}