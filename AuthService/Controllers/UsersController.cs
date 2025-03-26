using Microsoft.AspNetCore.Mvc;
using AuthService.Dtos;
using AuthService.Models;
using AuthService.Services;

namespace AuthService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : Controller
{
    private readonly IAuthService _userService;

    public UsersController(IAuthService userService)
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
