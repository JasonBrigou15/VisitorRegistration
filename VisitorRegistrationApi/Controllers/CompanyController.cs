using Microsoft.AspNetCore.Mvc;
using VisitorRegistrationApi.Dtos.Company;
using VisitorRegistrationService;

namespace VisitorRegistrationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService companyService;


        public CompanyController(CompanyService companyService)
        {
            this.companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            var companies = await companyService.GetAllCompanies();

            if (!companies.Any())
            {
                return NotFound("No companies found");
            }

            var companyDto = companies.Select(c => c.ToGetDto()).ToList();

            return Ok(companyDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID is not valid");
            }

            var company = await companyService.GetCompanyById(id);

            if (company == null)
            {
                return NotFound($"Company with ID {id} was not found.");
            }

            var companyDto = company.ToGetDto();

            return Ok(companyDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewCompany([FromBody] CreateCompanyDto createCompanyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await companyService.CreateNewCompany(createCompanyDto);

            return Ok("Company successfully created");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyDto updateCompanyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await companyService.UpdateCompany(updateCompanyDto);

            return Ok("Company successfully updated");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID is not valid");
            }

            var company = await companyService.GetCompanyById(id);

            if (company == null)
            {
                return NotFound($"Company with ID {id} was not found.");
            }

            if (company.IsDeleted)
            {
                return BadRequest("This company is already deleted");
            }

            await companyService.DeleteCompany(id);

            return NoContent();
        }
    }
}
