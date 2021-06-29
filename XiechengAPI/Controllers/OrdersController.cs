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
using XiechengAPI.ResourceParameters;
using XiechengAPI.Services;

namespace XiechengAPI.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController:ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRepository _touristRepository;
        private readonly IMapper _mapper;

        public OrdersController(IHttpContextAccessor httpContextAccessor, ITouristRepository touristRepository, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRepository = touristRepository;
            _mapper = mapper;
        }


        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrders([FromQuery] PaginationResourceParameters parameters)
        {
            // get user
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // get order
            var orders =await _touristRepository.GetOrdersByUserID(userId,parameters.PageNumber,parameters.PageSize);
            // return
            return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
        }

        [HttpGet("{orderId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
        {
            // get user
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var order =await _touristRepository.GetOrderById(orderId);

            return Ok(_mapper.Map<OrderDto>(order));
        }





    }
}
