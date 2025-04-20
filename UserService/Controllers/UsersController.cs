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
    public async Task<ActionResult<IEnumerable<User>>> GetCurrentUser()
    {
        var user = await _userService.GetCurrentUser();
        return Ok(user);
    }

    [Authorize]
    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetCurrentUserProducts()
    {
        var products = await _userService.GetCurrentUserProducts();
        return Ok(products);
    }

    [Authorize]
    [HttpGet("products/{productId}")]
    public async Task<ActionResult<GetUserProductInfoResponse>> GetUserProductInfo(int productId)
    {
        var productInfo = await _userService.GetUserProductInfo(productId);
        return Ok(productInfo);
    }

    [Authorize]
    [HttpPost("products")]
    public async Task<ActionResult> AddFreeProductToUser([FromBody] int productId)
    {
        await _userService.AddFreeProductToUser(productId);
        return Ok();
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
