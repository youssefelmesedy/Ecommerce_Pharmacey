using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.Application.Dtos.Products;
using Pharmacy.Application.Features.Products.Commands.CreateProduct;
using Pharmacy.Application.Features.Products.Commands.DeleteProduct;
using Pharmacy.Application.Features.Products.Commands.UpdateProduct;
using Pharmacy.Application.Features.Products.Qeuries.Get;
using Pharmacy.Application.Features.Products.Qeuries.GetById;
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
            var command = new GetPagedProductQuery(pageNumber, pageSize, categoryId, search);

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProduct([FromQuery] string? searchByName, CancellationToken cancellationToken)
        {
            var qurey = new GetAllProductQuery(searchByName);

            var result = await _mediator.Send(qurey, cancellationToken);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
        {
            var qurey = new GetProductByIdQuery(id);

            var result = await _mediator.Send(qurey, cancellationToken);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto, CancellationToken cancellationToken)
        {
            var command = new CreateProductCommand(dto);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto dto, CancellationToken cancellationToken)
        {
            var command = new UpdateProductCommand(dto);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteProductCommand(id);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
