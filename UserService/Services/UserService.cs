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

        public UserService(IUserRepo userRepo, IProductDataClient productClient)
        {
            _userRepo = userRepo;
            _productClient = productClient;
        }

        public async Task<User> GetUserById(int id)
        {
            var user = _userRepo.GetUserById(id);
            user.Products = await _productClient.GetProductsByUserId(id);

            return user;
        }

        public async Task<User> CreateUser(CreateUserRequest createUserRequest)
        {

            Random rnd = new Random();
            int uid = rnd.Next(100000000, 999999999);

            var user = new User
            {
                UID = uid.ToString(),
                Name = createUserRequest.Name,
                Email = createUserRequest.Email,
                Password = createUserRequest.Password
            };
            await _userRepo.CreateUser(user);
            _userRepo.SaveChanges();

            return user;
        }
    }
}
