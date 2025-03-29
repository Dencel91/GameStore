using System.Security.Claims;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;
using UserService.SyncData.Http;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IProductDataClient _productClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepo userRepo, IProductDataClient productClient, IHttpContextAccessor httpContextAccessor)
        {
            _userRepo = userRepo;
            _productClient = productClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<User> GetCurrentUser()
        {
            //var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return _userRepo.GetUserById(userId);
        }

        public async Task<User?> GetUserById(Guid id)
        {
            var user = await _userRepo.GetUserById(id);

            return user;
        }

        public async Task<User> AddUser(AddUserRequest createUserRequest)
        {
            var user = new User
            {
                Id = createUserRequest.Id,
                Name = createUserRequest.Name
            };

            await _userRepo.CreateUser(user);
            _userRepo.SaveChanges();

            return user;
        }
    }
}
