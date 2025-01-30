using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using StranglerPattern.ApiService.FeatureFlagging;

namespace StranglerPattern.ApiService.Controllers;

[ApiController]
[Route("name")]
[FeatureGate(MyFeatureFlags.NewNameApi)]
public class NameController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { Text = "New API!!! 😁😁😁😁" });
}