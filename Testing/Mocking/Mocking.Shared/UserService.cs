using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mocking.Shared
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<List<User>> GetAllAsync()
        {
            var list = (await _userRepository.GetAllAsync()).ToList();
            _logger.LogInformation("Retrieved {Count} users", list.Count);
            return list.Select(MapToDomain).ToList();
        }

        public async Task<User> CreateAsync(User user)
        {
            var userDto = new UserDto { UserName = user.UserName };
            var newUserDto = await _userRepository.CreateAsync(userDto);
            _logger.LogInformation("Created a new user with Id: {Id}", newUserDto.Id);
            return MapToDomain(newUserDto);
        }

        public async Task<User> GetByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogInformation("Unable to find a user with Id: {Id}", userId);
                return null;
            }

            _logger.LogInformation("Retrieved a user with Id: {Id}", userId);
            return MapToDomain(user);
        }

        private User MapToDomain(UserDto dto)
        {
            return new User { Id = Guid.Parse(dto.Id), UserName = dto.UserName };
        }
    }
}