using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.Application.Dtos.Categories;
using Pharmacy.Application.Features.Categories.Commands.CreateCategory;
using Pharmacy.Application.Features.Categories.Commands.DeleteCategory;
using Pharmacy.Application.Features.Categories.Commands.UpdateCategory;
using Pharmacy.Application.Features.Categories.Queries.Get;
using Pharmacy.Application.Features.Categories.Queries.GetById;
using Pharmacy.Application.Features.Categories.Queries.GetProductWithCategory;

namespace Pharmacy.Api.Controllers.ControllerCategories
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllCategories()
        {
            var result =
                await _mediator.Send(new GetCategoryQuery());

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("{category_Id:guid}")]
        public async Task<IActionResult> GetCategoryById(Guid category_Id, CancellationToken cancellation)
        {
            var result =
                await _mediator.Send(new GetCategoryByIdQuery(category_Id), cancellation);

                if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("GetCategoryIncludeProducts")]
        public async Task<IActionResult> GetCategoryIncludeProducts(CancellationToken cancellation)
        {
            var result =
                await _mediator.Send(new GetProductWithCategoryQuery(), cancellation);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("GetCategoryIncludeProducts/{category_Id}")]
        public async Task<IActionResult> GetCategoryIncludeProduct(Guid category_Id,CancellationToken cancellation)
        {
            var result =
                await _mediator.Send(new GetProductWithCategoryIdQuery(category_Id), cancellation);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto dto, CancellationToken cancellation)
        {
            var result = 
                await _mediator.Send(new CreateCategoryCommand(dto), cancellation);

            if (result.Data == null && result.Message != null)
                return Conflict(result);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
         }

        [HttpPut()]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto dto, CancellationToken cancellation)
        {
            var result = 
                await _mediator.Send(new UpdateCategoryCommand(dto), cancellation);

            if (!result.Succeeded)
                return NotFound(result);

            if (result.Data == null && result.Message != null)
                return Conflict(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{category_Id:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid category_Id, CancellationToken cancellation)
        {
            var result = 
                await _mediator.Send(new DeleteCategoryCommand(category_Id), cancellation);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }
    }
}
