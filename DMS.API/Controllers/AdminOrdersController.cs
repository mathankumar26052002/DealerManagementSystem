using DMS.Application.DTOs.Orders;
using DMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AdminOrdersController(
            IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(
                await _orderService.GetAllOrdersAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                return Ok(
                    await _orderService.GetOrderByIdAsync(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var userId = GetUserId();

                await _orderService.ApproveAsync(
                    id,
                    userId);

                return Ok(new
                {
                    message = "Order approved successfully."
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

        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> Reject(
    int id,
    [FromBody] RejectOrderRequestDto request)
        {
            try
            {
                var userId = GetUserId();

                await _orderService.RejectAsync(
                    id,
                    userId,
                    request.Reason);

                return Ok(new
                {
                    message = "Order rejected successfully."
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

        [HttpPost("{id:int}/dispatch")]
        public async Task<IActionResult> Dispatch(int id)
        {
            try
            {
                var userId = GetUserId();

                await _orderService.DispatchAsync(
                    id,
                    userId);

                return Ok(new
                {
                    message = "Order dispatched successfully."
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

        [HttpPost("{id:int}/deliver")]
        public async Task<IActionResult> Deliver(int id)
        {
            try
            {
                var userId = GetUserId();

                await _orderService.DeliverAsync(
                    id,
                    userId);

                return Ok(new
                {
                    message = "Order delivered successfully."
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



        private int GetUserId()
        {
            var userId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userId, out var id))
            {
                throw new UnauthorizedAccessException(
                    "User ID is missing from token.");
            }

            return id;
        }
    }
}
