using OpenTelemetry.Trace;

namespace KafkaSample.ServiceDefaults;

public class FilteringSampler(Sampler innerSampler) : Sampler
{
    public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
    {
        // Check for specific attributes (e.g., HTTP status code)
        if (samplingParameters.Name.Contains("kafka-flow.admin.telemetry"))
        {
            return new SamplingResult(SamplingDecision.Drop);
        }

        // Use the inner sampler's decision
        return innerSampler.ShouldSample(samplingParameters);
    }
}