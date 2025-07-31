using Microsoft.AspNetCore.Mvc;
using TickerQ.Utilities;
using TickerQ.Utilities.Interfaces.Managers;
using TickerQ.Utilities.Models.Ticker;
using TickerQExample.WebAPI.BackgroundJobs;

namespace TickerQExample.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ScheduleController(ITimeTickerManager<TimeTicker> timeTickerManager) : ControllerBase
{
    [HttpPost(Name = "ScheduleJob")]
    public async Task<IActionResult> ScheduleJob(TestObject obt)
    {
        var result = await timeTickerManager.AddAsync(new TimeTicker
        {
            Function = "WithObject",
            Description = null,
            Request = TickerHelper.CreateTickerRequest(obt),
            ExecutionTime = DateTime.Now.AddSeconds(3),
        });
        return result.IsSucceded ? Ok(result) : throw result.Exception;
    }
}