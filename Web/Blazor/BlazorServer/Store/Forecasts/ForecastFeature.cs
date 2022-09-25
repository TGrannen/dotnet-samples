using Fluxor;

namespace BlazorServer.Store.Forecasts;

public class ForecastFeature : Feature<ForecastState>
{
    public override string GetName() => "Forecasts";

    protected override ForecastState GetInitialState()
    {
        return new ForecastState(false, null, null);
    }
}