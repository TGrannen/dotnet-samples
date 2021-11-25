using System.Diagnostics;
using System.Threading.Tasks;

namespace Observability.WebAPI.Services
{
    public class ActivityService
    {
        public const string Name = "Observibility.WebAPI.ManualActivityTracing";
        private static readonly ActivitySource MyActivitySource = new(Name);

        public async Task Hello()
        {
            using var activity = MyActivitySource.StartActivity("SayHello");
            activity?.SetTag("foo", 1);
            activity?.SetTag("bar", "Hello, World!");
            activity?.SetTag("baz", new int[] { 1, 2, 3 });
            await Task.Delay(600);
        }

        public async Task Goodbye()
        {
            using var activity = MyActivitySource.StartActivity("SayGoodbye");
            activity?.SetTag("foo", 69);
            activity?.SetTag("bar", "Goodbye, World!");
            await Task.Delay(400);
        }
    }
}