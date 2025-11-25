using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using VisitorRegistrationData.Entities;
using VisitorRegistrationData.Interfaces;
using VisitorRegistrationService;
using VisitorRegistrationService.Dtos.Visitor;

namespace VisitorRegistrationTest.VisitorTests
{
    public class VisitorServiceTests
    {
        private readonly Mock<IVisitorRepository> visitorRepositoryMock;
        private readonly Mock<ICompanyRepository> companyRepositoryMock;

        private readonly Mock<IValidator<CreateVisitorDto>> createVisitorValidatorMock;
        private readonly Mock<IValidator<UpdateVisitorDto>> updateVisitorValidatorMock;

        private readonly VisitorService visitorService;

        public VisitorServiceTests()
        {
            visitorRepositoryMock = new Mock<IVisitorRepository>();
            companyRepositoryMock = new Mock<ICompanyRepository>();

            createVisitorValidatorMock = new Mock<IValidator<CreateVisitorDto>>();
            updateVisitorValidatorMock = new Mock<IValidator<UpdateVisitorDto>>();

            visitorService = new VisitorService(
                visitorRepositoryMock.Object,
                createVisitorValidatorMock.Object,
                companyRepositoryMock.Object,
                updateVisitorValidatorMock.Object);

        }

        // GetAllVisitors

        [Fact]
        public async Task GetAllVisitors_ShouldReturnList_WhenVisitorsExist()
        {
            // Arrange
            Company company = new Company { Id = 1, Name = "Microsoft" };

            var mockVisitors = new List<Visitor>
            {
                new Visitor { Id = 1, Firstname = "John", Lastname = "Doe", Email = "john.doe@test.com" },
                new Visitor { Id = 1, Firstname = "Jane", Lastname = "Smith", Email = "jane.smith@test.com", Company = company },
            };

            visitorRepositoryMock
                .Setup(repo => repo.GetAllVisitors())
                .ReturnsAsync(mockVisitors);

            // Act
            var result = await visitorService.GetAllVisitors();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Firstname.Should().Be("John");
            result[1].Firstname.Should().Be("Jane");
            result[1].CompanyName.Should().Be("Microsoft");
        }

        [Fact]
        public async Task GetAllVisitors_ShouldReturnEmptyList_WhenNoVisitorsExist()
        {
            // Arrange
            var mockVisitors = new List<Visitor>();

            visitorRepositoryMock
                .Setup(repo => repo.GetAllVisitors())
                .ReturnsAsync(mockVisitors);

            // Act
            var result = await visitorService.GetAllVisitors();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        // GetVisitorById

        [Fact]
        public async Task GetVisitorById_ShouldReturnVisitor_WhenVisitorExists()
        {
            // Arrange
            Company company = new Company { Id = 1, Name = "Microsoft" };

            var visitor = new Visitor
            {
                Id = 1,
                Firstname = "John",
                Lastname = "Doe",
                Email = "john.doe@test.com",
                CompanyId = 1,
                Company = company

            };

            visitorRepositoryMock
                .Setup(repo => repo.GetVisitorById(1))
                .ReturnsAsync(visitor);

            // Act
            var result = await visitorService.GetVisitorById(1);

            // Assert
            result.Should().NotBeNull();
            result.Firstname.Should().Be("John");
            result.Email.Should().Be("john.doe@test.com");
            result.CompanyName.Should().Be("Microsoft");
        }

        [Fact]
        public async Task GetVisitorById_ShouldReturnNull_WhenVisitorDoesNotExist()
        {
            // Arrange
            visitorRepositoryMock
                .Setup(repo => repo.GetVisitorById(1))
                .ReturnsAsync((Visitor?)null);

            // Act
            var result = await visitorService.GetVisitorById(1);

            // Assert
            result.Should().BeNull();
        }

        // CreateNewVisitor

        [Fact]
        public async Task CreateNewVisitor_ShouldCreateNewVisitor_WhenValidationPass()
        {
            // Arrange
            var createDto = new CreateVisitorDto
            {
                Firstname = "John",
                Lastname = "Doe",
                Email = "john.doe@test.com",
                CompanyName = "Microsoft"
            };

            var company = new Company { Id = 1, Name = "Microsoft" };

            createVisitorValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            companyRepositoryMock
                .Setup(repo => repo.GetCompanyByName("Microsoft"))
                .ReturnsAsync(company);

            // Act
            await visitorService.CreateNewVisitor(createDto);

            // Assert
            visitorRepositoryMock.Verify(repo => repo.CreateVisitor(It.Is<Visitor>(v =>
                v.Firstname == "John" &&
                v.Lastname == "Doe" &&
                v.Email == "john.doe@test.com" &&
                v.CompanyId == 1
                )), Times.Once);
        }

        [Fact]
        public async Task CreateVisitor_ShouldAllowEmptyCompanyName()
        {
            // Arrange
            var dto = new CreateVisitorDto
            {
                Firstname = "John",
                Lastname = "Doe",
                Email = "john@test.com",
                CompanyName = ""
            };

            createVisitorValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

            companyRepositoryMock
            .Setup(repo => repo.GetCompanyByName(It.IsAny<string>()))
            .ReturnsAsync((Company?)null);

            // Act
            await visitorService.CreateNewVisitor(dto);

            // Assert
            visitorRepositoryMock.Verify(repo => repo.CreateVisitor(It.IsAny<Visitor>()), Times.Once);
        }

        [Fact]
        public async Task CreateNewVisitor_ShouldNotCallCreateVisitor_WhenEmailIsInvalid()
        {
            // Arrange
            var createDto = new CreateVisitorDto
            {
                Firstname = "John",
                Lastname = "Doe",
                Email = "invalid-email",
                CompanyName = "Microsoft"
            };

            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("Email", "Invalid email format")
            };

            createVisitorValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult(failures));

            // Act
            await Assert.ThrowsAsync<ValidationException>(() => visitorService.CreateNewVisitor(createDto));

            // Assert
            visitorRepositoryMock.Verify(repo => repo.CreateVisitor(It.IsAny<Visitor>()), Times.Never);
        }

        [Fact]
        public async Task CreateVisitor_ShouldNotCreateVisitor_WhenEmailIsDuplicate()
        {
            // Arrange
            var createDto = new CreateVisitorDto
            {
                Firstname = "John",
                Lastname = "Doe",
                Email = "john.doe@test.com"
            };

            var existingVisitor = new Visitor
            {
                Id = 1,
                Firstname = "Existing",
                Lastname = "User",
                Email = "john.doe@test.com"
            };

            createVisitorValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            visitorRepositoryMock
                .Setup(repo => repo.GetVisitorByEmail(It.IsAny<string>()))
                .ReturnsAsync(existingVisitor);

            // Act && Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                  visitorService.CreateNewVisitor(createDto));

        }

        [Fact]
        public async Task CreateVisitor_ShouldThrowValidationException_WhenValidatorReturnsErrors()
        {
            // Arrange
            var dto = new CreateVisitorDto();

            createVisitorValidatorMock
                .Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult(new[]
                {new ValidationFailure("Email", "Invalid email")}));

            // Act
            await Assert.ThrowsAsync<ValidationException>(() => visitorService.CreateNewVisitor(dto));

            // Assert
            visitorRepositoryMock.Verify(repo => repo.CreateVisitor(It.IsAny<Visitor>()), Times.Never);
        }

        // UpdateVisitor

        [Fact]
        public async Task UpdateVisitor_ShouldUpdateVisitor_WhenValidationPass()
        {
            // Arrange
            var company = new Company { Id = 1, Name = "Microsoft" };

            var updateDto = new UpdateVisitorDto
            {
                Id = 1,
                Firstname = "John",
                Lastname = "Doe",
                Email = "john.doe@test.com",
                CompanyName = "Microsoft"


            };
            var existingVisitor = new Visitor
            {
                Id = 1,
                Firstname = "OldFirstName",
                Lastname = "OldLastName",
                Email = "oldEmail@test.com",
                CompanyId = 1
            };

            updateVisitorValidatorMock
               .Setup(v => v.ValidateAsync(updateDto, default))
               .ReturnsAsync(new ValidationResult());

            visitorRepositoryMock
                .Setup(repo => repo.GetVisitorById(1))
                .ReturnsAsync(existingVisitor);

            companyRepositoryMock
                .Setup(repo => repo.GetCompanyByName("Microsoft"))
                .ReturnsAsync(company);

            // Act
            await visitorService.UpdateVisitor(updateDto);

            // Assert
            updateVisitorValidatorMock.Verify(v => v.ValidateAsync(updateDto, default), Times.Once);
            visitorRepositoryMock.Verify(repo => repo.GetVisitorById(1), Times.Once);
            companyRepositoryMock.Verify(repo => repo.GetCompanyByName("Microsoft"), Times.Once);
            visitorRepositoryMock.Verify(repo => repo.UpdateVisitor(existingVisitor), Times.Once);

            existingVisitor.Firstname.Should().Be("John");
            existingVisitor.Lastname.Should().Be("Doe");
            existingVisitor.Email.Should().Be("john.doe@test.com");
            existingVisitor.CompanyId.Should().Be(1);
            existingVisitor.Company.Should().Be(company);
        }

        [Fact]
        public async Task UpdateVisitor_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var updateDto = new UpdateVisitorDto
            {
                Id = 1,
                Firstname = "",
                Lastname = "Doe",
                Email = "john.doe@test.com",
                CompanyName = "Microsoft"
            };

            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("FirstName", "First name is required")
            };

            updateVisitorValidatorMock
                .Setup(v => v.ValidateAsync(updateDto, default))
                .ReturnsAsync(new ValidationResult(failures));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => visitorService.UpdateVisitor(updateDto));
        }

        [Fact]
        public async Task UpdateVisitor_ShouldThrowException_WhenVisitorDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateVisitorDto
            {
                Id = 999,
                Firstname = "John",
                Lastname = "Doe",
                Email = "john.doe@test.com"
            };

            updateVisitorValidatorMock
                .Setup(v => v.ValidateAsync(updateDto, default))
                .ReturnsAsync(new ValidationResult());

            visitorRepositoryMock
                .Setup(repo => repo.GetVisitorById(updateDto.Id))
                .ReturnsAsync((Visitor?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => visitorService.UpdateVisitor(updateDto));
        }

        // DeleteVisitor

        [Fact]
        public async Task DeleteVisitor_ShouldCallRepositoryDeleteVisitor_WhenVisitorExists()
        {
            // Arrange
            var visitorId = 1;

            visitorRepositoryMock
                .Setup(repo => repo.GetVisitorById(visitorId))
                .ReturnsAsync(new Visitor { Id = visitorId, Firstname = "John", Lastname = "Doe", Email = "john.doe@test.com" });

            // Act
            await visitorService.DeleteVisitor(visitorId);

            // Assert
            visitorRepositoryMock.Verify(repo => repo.DeleteVisitor(visitorId), Times.Once);
        }

        [Fact]
        public async Task DeleteVisitor_ShouldThrowException_WhenIdIsInvalid()
        {
            // Arrange
            var visitorId = -1;

            visitorRepositoryMock
                .Setup(repo => repo.GetVisitorById(visitorId))
                .ReturnsAsync((Visitor?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => visitorService.DeleteVisitor(-1));
        }
    }
}