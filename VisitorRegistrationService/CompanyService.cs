using FluentValidation;
using VisitorRegistrationApi.Dtos.Company;
using VisitorRegistrationData.Entities;
using VisitorRegistrationData.Interfaces;
using VisitorRegistrationShared.Dtos.Appointments;
using VisitorRegistrationShared.Dtos.Company;

namespace VisitorRegistrationService
{
    public class CompanyService
    {
        private readonly ICompanyRepository companyRepository;

        // Validator
        private readonly IValidator<CreateCompanyDto> createCompanyDto;
        private readonly IValidator<UpdateCompanyDto> updateCompanyDto;

        public CompanyService(ICompanyRepository companyRepository, IValidator<CreateCompanyDto> createCompanyDto, 
            IValidator<UpdateCompanyDto> updateCompanyDto)
        {
            this.companyRepository = companyRepository;
            this.createCompanyDto = createCompanyDto;
            this.updateCompanyDto = updateCompanyDto;
        }

        public async Task<List<Company>> GetAllCompanies()
        {
            return await companyRepository.GetAllCompanies();
        }

        public async Task<Company?> GetCompanyById(int id)
        {
            return await companyRepository.GetCompanyById(id);
        }

        public async Task CreateNewCompany(CreateCompanyDto createCompanyDto)
        {
            var validationResult = await this.createCompanyDto.ValidateAsync(createCompanyDto);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var company = createCompanyDto.CreateDtoToEntity();

            await companyRepository.CreateCompany(company);
        }

        public async Task UpdateCompany(UpdateCompanyDto updateCompanyDto)
        {
            var validationResult = await this.updateCompanyDto.ValidateAsync(updateCompanyDto);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingCompany = await companyRepository.GetCompanyById(updateCompanyDto.Id);

            if (existingCompany == null)
                throw new Exception($"Company with ID {updateCompanyDto.Id} not found");

            updateCompanyDto.UpdateDtoToEntity(existingCompany!);

            await companyRepository.UpdateCompany(existingCompany!);
        }

        public async Task DeleteCompany(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid company ID");

            var existingCompany = await companyRepository.GetCompanyById(id);

            if (existingCompany == null)
                throw new Exception($"Company with ID {id} not found");

            await companyRepository.DeleteCompany(id);
        }
    }
}
