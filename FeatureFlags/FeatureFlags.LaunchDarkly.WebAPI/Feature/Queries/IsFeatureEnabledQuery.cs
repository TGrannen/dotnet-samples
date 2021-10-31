using System;
using System.Threading;
using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Context;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Models;
using FeatureFlags.LaunchDarkly.WebAPI.Feature.Users;
using LaunchDarkly.Sdk.Server.Interfaces;
using MediatR;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Queries
{
    public class IsFeatureEnabledQuery : IRequest<bool>
    {
        public Features Feature { get; set; }
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
            var key = ConvertToKey(request.Feature);
            var user = request.Context == null ? _contextualUserProvider.GetUser() : request.Context.Build();
            var result = _client.BoolVariation(key, user);
            return Task.FromResult(result);
        }

        private string ConvertToKey(Features feature)
        {
            return feature switch
            {
                Features.Feature1 => "demo-sample-feature",
                Features.Feature2 => "demo-sample-feature-2",
                _ => throw new ArgumentOutOfRangeException(nameof(feature), feature, null)
            };
        }
    }
}