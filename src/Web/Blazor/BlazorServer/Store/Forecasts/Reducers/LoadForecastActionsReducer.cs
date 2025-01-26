using BlazorServer.Store.Forecasts.Actions;
using Fluxor;

namespace BlazorServer.Store.Forecasts.Reducers;

public static class LoadForecastActionsReducer
{
    [ReducerMethod]
    public static ForecastState ReduceLoadForecastAction(ForecastState state, LoadForecastAction _)
    {
        return new ForecastState(true, null, null);
    }

    [ReducerMethod]
    public static ForecastState ReduceLoadForecastResultAction(ForecastState _, LoadForecastResultAction action)
    {
        return action.HasCurrentError
            ? new ForecastState(false, action.ErrorMessage, null)
            : new ForecastState(false, null, action.Forecasts);
    }
}