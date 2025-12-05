using Microsoft.AspNetCore.Mvc;
using VisitorRegistrationService;
using VisitorRegistrationService.Dtos.Visitor;
using VisitorRegistrationShared.Dtos.Visitor;

namespace VisitorRegistrationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitorController : ControllerBase
    {
        private readonly VisitorService visitorService;

        public VisitorController(VisitorService visitorService)
        {
            this.visitorService = visitorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVisitors()
        {
            var visitors = await visitorService.GetAllVisitors();

            if (!visitors.Any())
            {
                return NotFound("No visitors found");
            }

            return Ok(visitors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVisitorById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID is not valid");
            }

            var visitor = await visitorService.GetVisitorById(id);

            if (visitor == null)
            {
                return NotFound($"Visitor with ID {id} was not found");
            }

            return Ok(visitor);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewVisitor([FromBody] CreateVisitorDto createVisitorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await visitorService.CreateNewVisitor(createVisitorDto);

            return Ok("Visitor created successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVisitor([FromBody] UpdateVisitorDto updateVisitorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingVisitor = await visitorService.GetVisitorById(updateVisitorDto.Id);

            if (existingVisitor == null)
            {
                return NotFound($"Visitor with ID {updateVisitorDto.Id} was not found");
            }

            await visitorService.UpdateVisitor(updateVisitorDto);

            return Ok("Visitor updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisitor(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID is not valid");
            }

            var existingVisitor = await visitorService.GetVisitorById(id);

            if (existingVisitor == null)
            {
                return NotFound($"Visitor with ID {id} was not found");
            }

            await visitorService.DeleteVisitor(id);

            return Ok("Visitor deleted successfully");
        }
    }
}
