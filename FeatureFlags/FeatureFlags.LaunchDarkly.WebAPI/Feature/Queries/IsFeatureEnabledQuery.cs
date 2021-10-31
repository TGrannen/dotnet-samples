using System.Threading;
using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Context;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Users;
using LaunchDarkly.Sdk.Server.Interfaces;
using MediatR;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Queries
{
    public class IsFeatureEnabledQuery : IRequest<bool>
    {
        public string Key { get; set; }
        public IFeatureContext Context { get; set; }
    }

    public class GetBoolFeatureQueryHandler : IRequestHandler<IsFeatureEnabledQuery, bool>
    {
        private readonly IUserProvider _contextualUserProvider;
        private readonly ILdClient _client;

        public GetBoolFeatureQueryHandler(
            IUserProvider contextualUserProvider,
            ILdClient client)
        {
            _contextualUserProvider = contextualUserProvider;
            _client = client;
        }

        public Task<bool> Handle(IsFeatureEnabledQuery request, CancellationToken cancellationToken)
        {
            var user = request.Context == null ? _contextualUserProvider.GetUser() : request.Context.Build();
            var result = _client.BoolVariation(request.Key, user);
            return Task.FromResult(result);
        }
    }
}