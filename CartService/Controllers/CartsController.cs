using Microsoft.AspNetCore.Mvc;
using CartService.DTOs;
using CartService.Models;
using CartService.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CartService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET api/<CartController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> Get(int id)
        {
            try
            {
                var cart = await _cartService.GetCartById(id);

                if (cart == null)
                {
                    return NotFound();
                }

                return Ok(cart);
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("AddProduct")]
        public async Task<ActionResult<Cart>> AddProduct([FromBody]AddProductRequest addProductRequest)
        {
            try
            {
                var cart = await _cartService.AddProduct(addProductRequest.CartId, addProductRequest.ProductId);

                return Ok(cart);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            } 
        }

        [HttpDelete]
        [Route("RemoveProduct")]
        public async Task<ActionResult<Cart>> RemoveProduct([FromBody] AddProductRequest addProductRequest)
        {
            var cart = await _cartService.RemoveProduct(addProductRequest.CartId, addProductRequest.ProductId);

            return Ok(cart);
        }

        [HttpPost]
        [Route("StartPayment")]
        public async Task<ActionResult> StartPayment([FromBody] int cartId)
        {
            try
            {
                await _cartService.StartPayment(cartId);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("CompletePayment")]
        public async Task<ActionResult> CompletePayment([FromBody] int cartId)
        {
            await _cartService.CompletePayment(cartId);
            return Ok();
        }
    }
}
