using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.Models;
using UserService.SyncData.Http;

namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : Controller
{
    private readonly IUserRepo _userRepo;
    private readonly IProductDataClient _productClient;

    public UsersController(IUserRepo userRepo, IProductDataClient productClient)
    {
        _userRepo = userRepo;
        _productClient = productClient;
    }

    [HttpGet]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = _userRepo.GetUserById(id);
        user.Products = await _productClient.GetProductsByUserId(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // POST: UserController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(User user)
    {
        _userRepo.CreateUser(user);
        _userRepo.SaveChanges();

        return CreatedAtRoute(nameof(GetUserById), new { Id = user.Id }, user);
    }
}
