using System;
using FeatureFlags.LaunchDarkly.WebAPI.Features;

namespace FeatureFlags.LaunchDarkly.WebAPI.Services
{
    public class RandomUserService : IUserService
    {
        private static readonly Random Rng = new();

        private static readonly TestUser[] Users =
        {
            new() { Id = "1321", Name = "James" },
            new() { Id = "5412", Name = "Sarah" },
            new() { Id = "76534", Name = "David" },
            new() { Id = "3424", Name = "Mia" },
        };

        public TestUser GetUser()
        {
            var index = Rng.Next(0, Users.Length - 1);
            return Users[index];
        }
    }
}