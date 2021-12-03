using System;
using FeatureFlags.LaunchDarkly.WebAPI.Models;

namespace FeatureFlags.LaunchDarkly.WebAPI.Services
{
    public interface IUserService
    {
        TestUser GetUser();
    }

    public class RandomUserService : IUserService
    {
        private static readonly string[] Id =
        {
            "1321", "5412", "76534", "3424",
        };
        private static readonly string[] Names =
        {
            "James", "Sarah", "David", "Mia",
        };
        
        public TestUser GetUser()
        {
            var rng = new Random();
            var index = rng.Next(0, 3);
            var name = Names[index];
            var id = Id[index];
            return new TestUser
            {
                Id = id,
                Name = name
            };
        }
    }
}