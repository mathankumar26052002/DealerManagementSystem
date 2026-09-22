using DMS.Application.DTOs.Orders;
using DMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Dealer")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICurrentUserService _currentUserService;

        public OrdersController(
            IOrderService orderService,
            ICurrentUserService currentUserService)
        {
            _orderService = orderService;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDraft(
            [FromBody] CreateOrderRequestDto request)
        {
            try
            {
                var dealerId =
                    _currentUserService.GetDealerId();

                var order =
                    await _orderService.CreateDraftAsync(
                        dealerId,
                        request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = order.Id },
                    order);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders(
    [FromQuery] OrderQueryDto query)
        {
            var dealerId =
                _currentUserService.GetDealerId();

            return Ok(
                await _orderService.GetMyOrdersAsync(
                    dealerId,
                    query));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var dealerId =
                    _currentUserService.GetDealerId();

                return Ok(
                    await _orderService.GetMyOrderByIdAsync(
                        dealerId,
                        id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{orderId:int}/items/{itemId:int}")]
        public async Task<IActionResult> UpdateItem(
            int orderId,
            int itemId,
            [FromBody] UpdateOrderItemRequestDto request)
        {
            try
            {
                var dealerId =
                    _currentUserService.GetDealerId();

                return Ok(
                    await _orderService.UpdateItemAsync(
                        dealerId,
                        orderId,
                        itemId,
                        request));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{orderId:int}/items/{itemId:int}")]
        public async Task<IActionResult> RemoveItem(
            int orderId,
            int itemId)
        {
            try
            {
                var dealerId =
                    _currentUserService.GetDealerId();

                await _orderService.RemoveItemAsync(
                    dealerId,
                    orderId,
                    itemId);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("{id:int}/submit")]
        public async Task<IActionResult> Submit(int id)
        {
            try
            {
                var dealerId =
                    _currentUserService.GetDealerId();

                await _orderService.SubmitAsync(
                    dealerId,
                    id);

                return Ok(new
                {
                    message = "Order submitted successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var dealerId =
                    _currentUserService.GetDealerId();

                await _orderService.CancelAsync(
                    dealerId,
                    id);

                return Ok(new
                {
                    message = "Order cancelled successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
