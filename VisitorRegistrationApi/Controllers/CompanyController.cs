using FluentValidation;
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

        // Validators
        private readonly IValidator<CreateCompanyDto> createCompanyValidator;
        private readonly IValidator<UpdateCompanyDto> updateCompanyValidator;

        public CompanyController(CompanyService companyService, IValidator<CreateCompanyDto> createCompanyValidator, IValidator<UpdateCompanyDto> updateCompanyValidator)
        {
            this.companyService = companyService;
            this.createCompanyValidator = createCompanyValidator;
            this.updateCompanyValidator = updateCompanyValidator;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            var companies = await companyService.GetAllCompanies();

            if (!companies.Any()) return NotFound("No companies were found");

            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            if (id == 0) 
                return BadRequest("Invalid ID provided");

            var company = await companyService.GetCompanyById(id);

            if (company == null) return NotFound($"Company with ID {id} was not found");

            return Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewCompany([FromBody] CreateCompanyDto createCompanyDto)
        {
            var validationResult = await createCompanyValidator.ValidateAsync(createCompanyDto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var company = createCompanyDto.CreateDtoToEntity();

            await companyService.CreateNewCompany(company);

            return CreatedAtAction(nameof(GetCompanyById), new { id = company.Id }, company);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyDto updateCompanyDto)
        {
            var validationResult = await updateCompanyValidator.ValidateAsync(updateCompanyDto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var company = await companyService.GetCompanyById(updateCompanyDto.Id);

            if (company == null)
                return NotFound($"Company with ID {updateCompanyDto.Id} not found");

            updateCompanyDto.UpdateDtoToEntity(company);

            await companyService.UpdateCompany(company);

            return Ok(company);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            if (id == 0)
                return BadRequest("Invalid ID provided");

            await companyService.DeleteCompany(id);

            return Ok("Company succesfully removed");
        }
    }
}
