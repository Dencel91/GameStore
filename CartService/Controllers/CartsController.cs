﻿using Microsoft.AspNetCore.Mvc;
using CartService.DTOs;
using CartService.Services;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet("{id}")]
        public async Task<ActionResult<CartDto>> Get(int id)
        {
            var cart = await _cartService.GetCartById(id);

            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<CartDto?>> GetCurrentUserCart()
        {
            var cart = await _cartService.GetCurrentUserCart();
            return Ok(cart);
        }

        [HttpPost]
        [Route("add-product")]
        public async Task<ActionResult<CartDto>> AddProduct([FromBody]AddProductRequest addProductRequest)
        {
            var cart = await _cartService.AddProduct(addProductRequest.CartId, addProductRequest.ProductId);
            return Ok(cart);
        }

        [HttpDelete]
        [Route("remove-product")]
        public async Task<ActionResult<CartDto>> RemoveProduct([FromBody] RemoveProductRequest addProductRequest)
        {
            var cart = await _cartService.RemoveProduct(addProductRequest.CartId, addProductRequest.ProductId);
            return Ok(cart);
        }

        [Authorize]
        [HttpPost("merge")]
        public async Task<ActionResult<CartDto>> MergeCarts([FromBody] int cartId)
        {
            var cart = await _cartService.MergeCarts(cartId);
            return Ok(cart);
        }

        [Authorize]
        [HttpPost]
        [Route("payment/start")]
        public async Task<ActionResult> StartPayment([FromBody] int cartId)
        {
            await _cartService.StartPayment(cartId);
            return Ok();
        }

        [HttpPost]
        [Route("payment/complete")]
        public async Task<ActionResult> CompletePayment([FromBody] int cartId)
        {
            await _cartService.CompletePayment(cartId);
            return Ok();
        }
    }
}
