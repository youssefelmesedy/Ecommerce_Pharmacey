using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.Application.Dtos.Productes;
using Pharmacy.Application.Features.Products.Commands.CreateProduct;
using Pharmacy.Application.Features.Products.Qeuries.Pagenation;

namespace Pharmacy.Api.Controllers.ControllerProducts
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("paginated/{pageNumber}/{pageSize}")]
        public async Task<IActionResult> GetPaginated(int pageNumber, int pageSize, [FromQuery] Guid? categoryId, [FromQuery] string? search)
        {
            var command = new GetPagedProductCommand(pageNumber, pageSize, categoryId, search);

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            var command = new CreateProductCommand(dto);

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
