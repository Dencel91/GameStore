using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetCurrentUserInfo()
    {
        var user = await _userService.GetCurrentUserInfo();
        return Ok(user);
    }

    [HttpGet("{id}", Name = "GetUserById")]
    public async Task<ActionResult<User>> GetUserById(Guid id)
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
    public async Task<ActionResult> Create(AddUserRequest createUserRequest)
    {
        var user = await _userService.AddUser(createUserRequest);

        return CreatedAtRoute(nameof(GetUserById), new { Id = user.Id }, user);
    }
}
