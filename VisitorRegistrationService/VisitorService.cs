using FluentValidation;
using VisitorRegistrationData.Entities;
using VisitorRegistrationData.Interfaces;
using VisitorRegistrationService.Dtos.Visitor;

namespace VisitorRegistrationService
{
    public class VisitorService
    {
        private readonly IVisitorRepository visitorRepository;
        private readonly ICompanyRepository companyRepository;

        private readonly IValidator<CreateVisitorDto> createVisitorDtoValidator;
        private readonly IValidator<UpdateVisitorDto> updateVisitorDtoValidator;

        public VisitorService(IVisitorRepository visitorRepository, IValidator<CreateVisitorDto> createVisitorDtoValidator, ICompanyRepository companyRepository,
            IValidator<UpdateVisitorDto> updateVisitorDtoValidator)
        {
            this.visitorRepository = visitorRepository;
            this.createVisitorDtoValidator = createVisitorDtoValidator;
            this.companyRepository = companyRepository;
            this.updateVisitorDtoValidator = updateVisitorDtoValidator;
        }

        public async Task<List<GetVisitorDto>> GetAllVisitors()
        {
            var visitors = await visitorRepository.GetAllVisitors();
            return visitors.Select(v => v.ToGetDto()).ToList();
        }

        public async Task<GetVisitorDto?> GetVisitorById(int id)
        {
            var visitor = await visitorRepository.GetVisitorById(id);
            return visitor?.ToGetDto();
        }

        public async Task CreateNewVisitor(CreateVisitorDto createVisitorDto)
        {
            var validationResult = await createVisitorDtoValidator.ValidateAsync(createVisitorDto);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingVisitor = await visitorRepository.GetVisitorByEmail(createVisitorDto.Email);

            if (existingVisitor != null)
                throw new ValidationException("A visitor with this email already exists.");

            Company? company = null;

            if (!string.IsNullOrWhiteSpace(createVisitorDto.CompanyName))
            {
                company = await companyRepository.GetCompanyByName(createVisitorDto.CompanyName);
            }

            var visitor = createVisitorDto.CreateDtoToEntity(company);

            await visitorRepository.CreateVisitor(visitor);
        }

        public async Task UpdateVisitor(UpdateVisitorDto updateVisitorDto)
        {
            var validationResult = await updateVisitorDtoValidator.ValidateAsync(updateVisitorDto);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingVisitor = await visitorRepository.GetVisitorById(updateVisitorDto.Id);

            if (existingVisitor == null)
                throw new Exception($"Visitor with ID {updateVisitorDto.Id} not found");

            var visitorWithSameEmail = await visitorRepository.GetVisitorByEmail(updateVisitorDto.Email);

            if (visitorWithSameEmail != null && visitorWithSameEmail.Id != updateVisitorDto.Id)
                throw new ValidationException("A visitor with this email already exists");

            Company? company = null;

            if (!string.IsNullOrWhiteSpace(updateVisitorDto.CompanyName))
            {
                company = await companyRepository.GetCompanyByName(updateVisitorDto.CompanyName);
            }

            updateVisitorDto.UpdateDtoToEntity(existingVisitor, company);

            await visitorRepository.UpdateVisitor(existingVisitor);
        }

        public async Task DeleteVisitor(int id)
        {
            if (id <= 0)
                throw new Exception("Invalid visitor ID");

            await visitorRepository.DeleteVisitor(id);
        }
    }
}
