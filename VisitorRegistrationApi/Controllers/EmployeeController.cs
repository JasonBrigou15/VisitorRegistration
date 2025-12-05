using Microsoft.AspNetCore.Mvc;
using VisitorRegistrationService;
using VisitorRegistrationService.Dtos.Employee;
using VisitorRegistrationShared.Dtos.Employee;

namespace VisitorRegistrationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService employeeService;

        public EmployeeController(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await employeeService.GetAllEmployees();

            if (!employees.Any())
            {
                return NotFound("No employees found");
            }

            var employeeDto = employees.Select(e => e.ToGetDto()).ToList();

            return Ok(employeeDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID is not valid");
            }

            var employee = await employeeService.GetEmployeeById(id);

            if (employee == null)
            {
                return NotFound($"Employee with ID {id} was not found");
            }

            var employeeDto = employee.ToGetDto();

            return Ok(employeeDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await employeeService.CreateNewEmployee(createEmployeeDto);

            return Ok("Employee successfully created");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await employeeService.UpdateEmployee(updateEmployeeDto);

            return Ok("Employee successfully updated");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID is not valid");
            }

            var employee = await employeeService.GetEmployeeById(id);

            if (employee == null)
            {
                return NotFound($"Employee with {id} was not found");
            }

            if (employee.IsDeleted)
            {
                return BadRequest("This employee is already deleted");
            }

            await employeeService.DeleteEmployee(id);

            return NoContent();
        }
    }
}
