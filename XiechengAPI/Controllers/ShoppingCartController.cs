using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XiechengAPI.Dtos;
using XiechengAPI.Helper;
using XiechengAPI.Moldes;
using XiechengAPI.Services;

namespace XiechengAPI.Controllers
{
    [ApiController]
    [Route("api/shoppingCart")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRepository _touristRepository;
        private readonly IMapper _mapper;

        public ShoppingCartController(IHttpContextAccessor httpContextAccessor, ITouristRepository touristRepository, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRepository = touristRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetShoppingCart()
        {
            // get user id
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // use this id get shopping cart
            var shoppingCartFromRepo = await _touristRepository.GetShoppingCartByUserId(userId);
            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCartFromRepo));
        }


        [HttpPost("items")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CreateShoppingCartItem([FromBody] AddShoppingCartItemDto addShoppingCart)
        {
            // get user id
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // use this id get shopping cart
            var shoppingCartFromRepo = await _touristRepository.GetShoppingCartByUserId(userId);
            // create line item
            var touristRouteFromRepo = await _touristRepository.GetTouristRouteAsync(addShoppingCart.TouristRouteId);
            if (touristRouteFromRepo == null)
            {
                return NotFound("Not found the route in our database");
            }
            var lineItem = new LineItem()
            {
                TouristRouteId = addShoppingCart.TouristRouteId,
                ShoppingCartId = shoppingCartFromRepo.Id,
                OriPrice = touristRouteFromRepo.OriPrice,
                DiscountPresent = touristRouteFromRepo.DiscountPresent
            };
            // save to the database
            await _touristRepository.AddShoppingCartItem(lineItem);
            await _touristRepository.SaveAsync();
            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCartFromRepo));
        }

        [HttpDelete("items/{itemId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItem([FromRoute] int itemId)
        {
            var lineItem = await _touristRepository.GetShoppingCartItemById(itemId);
            if (lineItem == null)
            {
                return NotFound("Route can't found in shopping cart");
            }

            _touristRepository.DeleteShoppingCartItem(lineItem);
            await _touristRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("items/({itemIds})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> RemoveShoppingCartItems(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] [FromRoute]
            IEnumerable<int> itemIds)
        {
            var lineItems = await _touristRepository.GetShoppingCartItemsByListAsync(itemIds);
            _touristRepository.DeleteShoppingCartItems(lineItems);
            await _touristRepository.SaveAsync();
            return NoContent();
        }

        [HttpPost("checkout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Checkout()
        {
            // get user id
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // use this id get shopping cart
            var shoppingCart = await _touristRepository.GetShoppingCartByUserId(userId);
            // create an order
            var order = new Order()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                State = OrderStateEnum.Pending,
                OrderItems = shoppingCart.ShoppingCartItems,
                CreateDateUTC = DateTime.Now
            };
            // clear shoppingCart
            shoppingCart.ShoppingCartItems = null;
            // save to database
            await _touristRepository.AddOrderAsync(order);
            await _touristRepository.SaveAsync();
            // return
            return Ok(_mapper.Map<OrderDto>(order));
        }
    }

}
