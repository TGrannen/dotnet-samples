using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace StranglerPattern.ApiService.FeatureFlagging;

public class FeatureFlagMiddleware(RequestDelegate next, IFeatureManager featureManager, ILogger<FeatureFlagMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        var featureGateMetadata = endpoint?.Metadata.GetMetadata<FeatureGateAttribute>();
        var featureName = featureGateMetadata?.Features.FirstOrDefault();
        if (featureName == null || await featureManager.IsEnabledAsync(featureName))
        {
            await next(context);
            return;
        }

        var originalPath = context.Request.Path.Value;
        var redirectPath = $"/proxy{originalPath}";
        logger.LogInformation("Feature '{FeatureName}' is disabled. Redirecting {OriginalPath} -> {RedirectPath}", featureName, originalPath,
            redirectPath);

        context.Response.Redirect(redirectPath, permanent: false);
    }
}