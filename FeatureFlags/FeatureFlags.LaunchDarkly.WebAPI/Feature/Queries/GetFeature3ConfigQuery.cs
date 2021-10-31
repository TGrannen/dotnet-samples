using System.Threading;
using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Context;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Users;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server.Interfaces;
using MediatR;
using Newtonsoft.Json;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Queries
{
    public class GetFeature3ConfigQuery : IRequest<GetFeature3ConfigQueryVm>
    {
        public IFeatureContext Context { get; set; }
    }

    public class GetFeature3ConfigQueryHandler : IRequestHandler<GetFeature3ConfigQuery, GetFeature3ConfigQueryVm>
    {
        private readonly IUserProvider _provider;
        private readonly ILdClient _client;

        public GetFeature3ConfigQueryHandler(IUserProvider provider, ILdClient client)
        {
            _provider = provider;
            _client = client;
        }

        public Task<GetFeature3ConfigQueryVm> Handle(GetFeature3ConfigQuery request, CancellationToken cancellationToken)
        {
            var user = request.Context == null ? _provider.GetUser() : request.Context.Build();
            var json = _client.JsonVariation("demo-json-feature", user, new LdValue());
            return Task.FromResult(JsonConvert.DeserializeObject<GetFeature3ConfigQueryVm>(json.ToJsonString()));
        }
    }
    
    public class GetFeature3ConfigQueryVm
    {
        public string Name { get; set; }
    }
}