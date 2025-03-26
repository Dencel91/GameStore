using Microsoft.AspNetCore.Mvc;
using UserService.Dtos;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}", Name = "GetUserById")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = await _userService.GetUserById(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // POST: UserController/Create
    [HttpPost]
    public async Task<ActionResult> Create(CreateUserRequest createUserRequest)
    {
        var user = await _userService.CreateUser(createUserRequest);

        return CreatedAtRoute(nameof(GetUserById), new { Id = user.Id }, user);
    }
}
