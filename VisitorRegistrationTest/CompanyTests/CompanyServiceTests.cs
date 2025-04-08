using FluentAssertions;
using FluentValidation;
using Moq;
using VisitorRegistrationApi.Dtos.Company;
using VisitorRegistrationData.Entities;
using VisitorRegistrationData.Interfaces;
using VisitorRegistrationService;
using VisitorRegistrationShared.Extensions;

namespace VisitorRegistrationTest
{
    public class CompanyServiceTests
    {
        private readonly Mock<ICompanyRepository> companyRepoMock;

        private readonly Mock<IValidator<CreateCompanyDto>> createCompanyValidatorMock;
        private readonly Mock<IValidator<UpdateCompanyDto>> updateCompanyValidatorMock;

        private readonly CompanyService companyService;

        public CompanyServiceTests()
        {
            companyRepoMock = new Mock<ICompanyRepository>();

            createCompanyValidatorMock = new Mock<IValidator<CreateCompanyDto>>();
            updateCompanyValidatorMock = new Mock<IValidator<UpdateCompanyDto>>();

            companyService = new CompanyService(
                companyRepoMock.Object,
                createCompanyValidatorMock.Object,
                updateCompanyValidatorMock.Object);
        }

        // GetAllCompanies

        [Fact]
        public async Task GetAllCompanies_ShouldReturnList_WhenCompaniesExist()
        {
            // Arrange
            var mockCompanies = new List<Company>
            {
                new Company { Id = 1, Name = "Test Company A" },
                new Company { Id = 2, Name = "Test Company B" }
            };

            companyRepoMock
                .Setup(repo => repo.GetAllCompanies())
                .ReturnsAsync(mockCompanies);

            // Act
            var result = await companyService.GetAllCompanies();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Test Company A");
            result[1].Name.Should().Be("Test Company B");
        }

        [Fact]
        public async Task GetAllCompanies_ShouldReturnEmptyList_WhenNoCompaniesExist()
        {
            // Arrange
            companyRepoMock
                .Setup(repo => repo.GetAllCompanies())
                .ReturnsAsync(new List<Company>());

            // Act
            var result = await companyService.GetAllCompanies();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        // GetCompanyById

        [Fact]
        public async Task GetCompanyById_ShouldReturnCompany_WhenCompanyExists()
        {
            // Arrange
            var company = new Company { Id = 1, Name = "Test Co" };

            companyRepoMock
                .Setup(repo => repo.GetCompanyById(1))
                .ReturnsAsync(company);

            // Act
            var result = await companyService.GetCompanyById(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("Test Co");
        }

        [Fact]
        public async Task GetCompanyById_ShouldReturnNull_WhenCompanyDoesNotExist()
        {
            // Arrange
            companyRepoMock
                .Setup(repo => repo.GetCompanyById(99))
                .ReturnsAsync((Company?)null);

            // Act
            var result = await companyService.GetCompanyById(99);

            // Assert
            result.Should().BeNull();
        }

        // CreateNewCompany

        [Fact]
        public async Task CreateNewCompany_ShouldCreateNewCompany_WhenValidationPasses()
        {
            // Arrange
            var createDto = new CreateCompanyDto { Name = "valid Co" };

            createCompanyValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var mappedCompany = new Company { Name = "Valid Co" };

            companyRepoMock
                .Setup(r => r.CreateCompany(It.IsAny<Company>()))
                .ReturnsAsync(mappedCompany);

            // Act
            await companyService.CreateNewCompany(createDto);

            // Assert
            createCompanyValidatorMock.Verify(validation => validation.ValidateAsync(createDto, default), Times.Once);
            companyRepoMock.Verify(repo => repo.CreateCompany(It.Is<Company>(company => company.Name == "Valid Co")), Times.Once);
        }

        [Fact]
        public async Task CreateNewCompany_ShouldThrowValidationException_WhenNoNameIsGiven()
        {
            // Arrange
            var createDto = new CreateCompanyDto { Name = "" };
            var expectedErrorMessage = "Company name is required";

            createCompanyValidatorMock
                .Setup(validator => validator.ValidateAsync(createDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult
                {
                    Errors = { new FluentValidation.Results.ValidationFailure("Name", expectedErrorMessage) }
                });

            // Act & Assert
            await FluentActions.Invoking(() => companyService.CreateNewCompany(createDto))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMessage($"*{expectedErrorMessage}*");
        }

        [Fact]
        public async Task CreateNewCompany_ShouldReturnValidationException_WhenNameIsInvalid()
        {
            // Arrange
            var addCompanyModel = new CreateCompanyDto { Name = "Compan||y @" };

            createCompanyValidatorMock
                .Setup(validator => validator.ValidateAsync(addCompanyModel, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult
                {
                    Errors = { new FluentValidation.Results.ValidationFailure("Name", "Company name contains invalid characters.") }
                });

            // Act & Assert
            await FluentActions.Invoking(() => companyService.CreateNewCompany(addCompanyModel))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMessage("*Company name contains invalid characters.*");
        }

        [Fact]
        public async Task CreateNewCompany_ShouldThrowValidationException_WhenCompanyNameAlreadyExists()
        {
            // Arrange
            var dto = new CreateCompanyDto { Name = "Company A" };

            var validationResult = new FluentValidation.Results.ValidationResult(
                new List<FluentValidation.Results.ValidationFailure>
                {
                    new FluentValidation.Results.ValidationFailure("Name", "A company with this name already exists.")
                });

            createCompanyValidatorMock
                .Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(validationResult);

            // Act
            var act = async () => await companyService.CreateNewCompany(dto);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*A company with this name already exists.*");

            companyRepoMock.Verify(repo => repo.CreateCompany(It.IsAny<Company>()), Times.Never);
        }

        // UpdateCompany

        [Fact]
        public async Task UpdateCompany_ShouldUpdateCompany_WhenValidationPasses()
        {
            // Arrange
            var dto = new UpdateCompanyDto { Id = 1, Name = "Updated Co" };
            var existingCompany = new Company { Id = 1, Name = "Old Co" };

            updateCompanyValidatorMock
                .Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            companyRepoMock
                .Setup(r => r.GetCompanyById(dto.Id))
                .ReturnsAsync(existingCompany);

            // Act
            await companyService.UpdateCompany(dto);

            // Assert
            companyRepoMock.Verify(repo => repo.UpdateCompany(It.Is<Company>(company =>
                company.Id == dto.Id && company.Name == "Updated Co")), Times.Once);

        }

        [Fact]
        public async Task UpdateCompany_ShouldThrowValidationException_WhenNameIsEmpty()
        {
            // Arrange
            var dto = new UpdateCompanyDto { Id = 1, Name = "" };

            var validationResult = new FluentValidation.Results.ValidationResult(
                new List<FluentValidation.Results.ValidationFailure>
                {
                    new FluentValidation.Results.ValidationFailure("Name", "Company name is required")
                });

            updateCompanyValidatorMock
                .Setup(validation => validation.ValidateAsync(dto, default))
                .ReturnsAsync(validationResult);

            // Act
            var act = async () => await companyService.UpdateCompany(dto);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Company name is required*");

            companyRepoMock.Verify(repo => repo.UpdateCompany(It.IsAny<Company>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCompany_ShouldThrowException_WhenCompanyDoesNotExist()
        {
            // Arrange
            var dto = new UpdateCompanyDto { Id = 999, Name = "Ghost Co" };

            updateCompanyValidatorMock
                .Setup(validation => validation.ValidateAsync(dto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            companyRepoMock
                .Setup(repo => repo.GetCompanyById(dto.Id))
                .ReturnsAsync((Company?)null);

            // Act
            var act = async () => await companyService.UpdateCompany(dto);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Company with ID 999 not found");

            companyRepoMock.Verify(repo => repo.UpdateCompany(It.IsAny<Company>()), Times.Never);
        }

        // DeleteCompany

        [Fact]
        public async Task DeleteCompany_ShouldCallRepository_WhenIdIsValid()
        {
            // Arrange
            var id = 1;

            companyRepoMock
                .Setup(repo => repo.DeleteCompany(id))
                .Returns(Task.CompletedTask);

            // Act
            await companyService.DeleteCompany(id);

            // Assert
            companyRepoMock.Verify(repo => repo.DeleteCompany(id), Times.Once);
        }
    }
}