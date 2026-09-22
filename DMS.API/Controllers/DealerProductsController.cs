using DMS.Application.DTOs.Common;
using DMS.Application.DTOs.Products;
using DMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    [ApiController]
    [Route("api/Products")]
    [Authorize(Roles = "Dealer")]
    public class DealerProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public DealerProductsController(
            IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("catalog")]
        public async Task<ActionResult<PagedResponseDto<ProductResponseDto>>> GetCatalog(
            [FromQuery] ProductCatalogQueryDto query)
        {
            var result = await _productService.GetCatalogAsync(
                query.Search,
                query.PageNumber,
                query.PageSize);

            return Ok(result);
        }
    }
}
