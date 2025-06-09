using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using VisitorRegistrationData.Entities;
using VisitorRegistrationData.Interfaces;
using VisitorRegistrationService;
using VisitorRegistrationService.Dtos.Employee;
using VisitorRegistrationShared.Extensions;

namespace VisitorRegistrationTest.CompanyTests
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> employeeRepoMock;
        private readonly Mock<ICompanyRepository> companyRepoMock;


        private readonly Mock<IValidator<CreateEmployeeDto>> createEmployeeValidatorMock;
        private readonly Mock<IValidator<UpdateEmployeeDto>> updateEmployeeValidatorMock;

        private readonly EmployeeService employeeService;

        public EmployeeServiceTests()
        {
            employeeRepoMock = new Mock<IEmployeeRepository>();
            companyRepoMock = new Mock<ICompanyRepository>();

            createEmployeeValidatorMock = new Mock<IValidator<CreateEmployeeDto>>();
            updateEmployeeValidatorMock = new Mock<IValidator<UpdateEmployeeDto>>();

            employeeService = new EmployeeService(
                employeeRepoMock.Object,
                companyRepoMock.Object,
                createEmployeeValidatorMock.Object,
                updateEmployeeValidatorMock.Object);
        }

        // GetAllCompanies

        [Fact]
        public async Task GetAllEmployees_ShouldReturnList_WhenEmployeesExist()
        {
            // Arrange
            var mockCompany = new Company { Id = 1, Name = "Test Company" };

            var mockEmployees = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "John", LastName = "Doe", CompanyId = 1, Company = mockCompany },
                new Employee { Id = 2, FirstName = "Jane", LastName = "Smith", CompanyId = 2, Company = new Company { Id = 2, Name = "Another Co" } }
            };

            employeeRepoMock
                .Setup(repo => repo.GetAllEmployees())
                .ReturnsAsync(mockEmployees);

            // Act
            var result = await employeeService.GetAllEmployees();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].FirstName.Should().Be("John");
            result[0].Company.Name.Should().Be("Test Company");
            result[1].FirstName.Should().Be("Jane");
            result[1].Company.Name.Should().Be("Another Co");
        }

        [Fact]
        public async Task GetAllEmployees_ShouldReturnEmptyList_WhenNoEmployeesExist()
        {
            // Arrange
            employeeRepoMock
                .Setup(repo => repo.GetAllEmployees())
                .ReturnsAsync(new List<Employee>());

            // Act
            var result = await employeeService.GetAllEmployees();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        // GetEmployeeById

        [Fact]
        public async Task GetEmployeeById_ShouldReturnEmployee_WhenEmployeeExists()
        {
            // Arrange
            var mockCompany = new Company { Id = 1, Name = "Test Co" };
            var employee = new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                CompanyId = 1,
                Company = mockCompany
            };

            employeeRepoMock
                .Setup(repo => repo.GetEmployeeById(1))
                .ReturnsAsync(employee);

            // Act
            var result = await employeeService.GetEmployeeById(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.FirstName.Should().Be("John");
            result.LastName.Should().Be("Doe");
            result.CompanyId.Should().Be(1);
        }

        [Fact]
        public async Task GetEmployeeById_ShouldReturnNull_WhenEmployeeDoesNotExist()
        {
            // Arrange
            employeeRepoMock
                .Setup(repo => repo.GetEmployeeById(99))
                .ReturnsAsync((Employee?)null);

            // Act
            var result = await employeeService.GetEmployeeById(99);

            // Assert
            result.Should().BeNull();
        }

        // CreateNewEmployee

        [Fact]
        public async Task CreateNewEmployee_ShouldCreateNewEmployee_WhenValidationPass()
        {
            // Arrange
            var createDto = new CreateEmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Title = "Manager",
                CompanyId = 1
            };

            var company = new Company { Id = 1, Name = "Test Co" };

            var expectedEmail = StringExtensions.ToEmailAddress(
                createDto.FirstName,
                createDto.LastName,
                createDto.Title,
                company.Name);

            createEmployeeValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            companyRepoMock
                .Setup(r => r.GetCompanyById(createDto.CompanyId))
                .ReturnsAsync(company);

            employeeRepoMock
                .Setup(r => r.CreateEmployee(It.IsAny<Employee>()));

            // Act
            await employeeService.CreateNewEmployee(createDto);

            // Assert
            createEmployeeValidatorMock.Verify(v => v.ValidateAsync(createDto, default), Times.Once);
            companyRepoMock.Verify(r => r.GetCompanyById(createDto.CompanyId), Times.Once);
            employeeRepoMock.Verify(r => r.CreateEmployee(It.Is<Employee>(e =>
                e.FirstName == "John" &&
                e.LastName == "Doe" &&
                e.Title == "Manager" &&
                e.CompanyId == 1 &&
                e.CompanyEmail.StartsWith($"{createDto.FirstName.ToLowerInvariant()}.{createDto.LastName.ToLowerInvariant()}.{createDto.Title.ToLowerInvariant()}@")
            )), Times.Once);
        }

        [Fact]
        public async Task CreateNewEmployee_ShouldNotCallCreateEmployee_WhenValidationFails()
        {
            // Arrange
            var createDto = new CreateEmployeeDto { FirstName = "" };
            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("FirstName", "First name is required")
            };

            createEmployeeValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult(failures));

            // Act
            await Assert.ThrowsAsync<ValidationException>(() => employeeService.CreateNewEmployee(createDto));

            // Assert
            employeeRepoMock.Verify(r => r.CreateEmployee(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public async Task CreateNewEmployee_ShouldThrowArgumentException_WhenCompanyNotFound()
        {
            // Arrange
            var createDto = new CreateEmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Title = "Manager",
                CompanyId = 999
            };

            createEmployeeValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            companyRepoMock
                .Setup(r => r.GetCompanyById(createDto.CompanyId))
                .ReturnsAsync((Company?)null);

            // Act & Assert
            await FluentActions.Invoking(() => employeeService.CreateNewEmployee(createDto))
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("*Selected company does not exist*");

            employeeRepoMock.Verify(r => r.CreateEmployee(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public async Task CreateNewEmployee_ShouldThrowValidationException_WhenCompanyIdIsInvalid()
        {
            // Arrange
            var createDto = new CreateEmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Title = "Manager",
                CompanyId = 0  // Invalid
            };

            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("CompanyId", "A valid company must be selected")
            };

            createEmployeeValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult(failures));

            // Act & Assert
            await FluentActions.Invoking(() => employeeService.CreateNewEmployee(createDto))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMessage("*A valid company must be selected*");

            employeeRepoMock.Verify(r => r.CreateEmployee(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public async Task CreateNewEmployee_ShouldThrowValidationException_WhenFirstNameIsTooLong()
        {
            // Arrange
            var longName = new string('A', 100);
            var createDto = new CreateEmployeeDto
            {
                FirstName = longName,
                LastName = "Doe",
                Title = "Manager",
                CompanyId = 1
            };

            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("FirstName", "First name cannot be longer than 50 characters")
            };

            createEmployeeValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult(failures));

            // Act & Assert
            await FluentActions.Invoking(() => employeeService.CreateNewEmployee(createDto))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMessage("*First name cannot be longer than 50 characters*");

            employeeRepoMock.Verify(r => r.CreateEmployee(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public async Task CreateNewEmployee_ShouldCreateEmployee_WithFormattedEmail_WhenCompanyNameHasSpecialChars()
        {
            // Arrange
            var createDto = new CreateEmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Title = "Manager",
                CompanyId = 1
            };

            var company = new Company { Id = 1, Name = "Test Co & Sons!" };
            var expectedEmail = StringExtensions.ToEmailAddress(
                createDto.FirstName,
                createDto.LastName,
                createDto.Title,
                company.Name);

            createEmployeeValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            companyRepoMock
                .Setup(r => r.GetCompanyById(createDto.CompanyId))
                .ReturnsAsync(company);

            employeeRepoMock
                .Setup(r => r.CreateEmployee(It.IsAny<Employee>()));

            // Act
            await employeeService.CreateNewEmployee(createDto);

            // Assert
            employeeRepoMock.Verify(r => r.CreateEmployee(It.Is<Employee>(e =>
                e.CompanyEmail.StartsWith($"{createDto.FirstName.ToLowerInvariant()}" +
                $".{createDto.LastName.ToLowerInvariant()}.{createDto.Title.ToLowerInvariant()}@")
            )), Times.Once);
        }

        // UpdateEmployee

        [Fact]
        public async Task UpdateEmployee_ShouldUpdateEmployee_WhenValidationPasses()
        {
            // Arrange
            var updateDto = new UpdateEmployeeDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Title = "Senior Manager",
                CompanyId = 1
            };

            var existingEmployee = new Employee
            {
                Id = 1,
                FirstName = "Old",
                LastName = "Name",
                Title = "Old Title",
                CompanyId = 2,
                Company = new Company { Id = 2, Name = "Old Co" }
            };

            var company = new Company { Id = 1, Name = "New Co" };

            updateEmployeeValidatorMock
                .Setup(v => v.ValidateAsync(updateDto, default))
                .ReturnsAsync(new ValidationResult());

            employeeRepoMock
                .Setup(r => r.GetEmployeeById(updateDto.Id))
                .ReturnsAsync(existingEmployee);

            companyRepoMock
                .Setup(r => r.GetCompanyById(updateDto.CompanyId))
                .ReturnsAsync(company);

            employeeRepoMock
                .Setup(r => r.UpdateEmployee(It.IsAny<Employee>()));

            // Act
            await employeeService.UpdateEmployee(updateDto);

            // Assert
            updateEmployeeValidatorMock.Verify(v => v.ValidateAsync(updateDto, default), Times.Once);
            employeeRepoMock.Verify(r => r.GetEmployeeById(updateDto.Id), Times.Once);
            companyRepoMock.Verify(r => r.GetCompanyById(updateDto.CompanyId), Times.Once);
            employeeRepoMock.Verify(r => r.UpdateEmployee(It.Is<Employee>(e =>
                e.Id == updateDto.Id &&
                e.FirstName == "John" &&
                e.LastName == "Doe" &&
                e.Title == "Senior Manager" &&
                e.CompanyId == 1 &&
                e.Company == company
            )), Times.Once);

            existingEmployee.FirstName.Should().Be("John");
            existingEmployee.LastName.Should().Be("Doe");
            existingEmployee.Title.Should().Be("Senior Manager");
            existingEmployee.CompanyId.Should().Be(1);
            existingEmployee.Company.Should().Be(company);
        }

        [Fact]
        public async Task UpdateEmployee_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var updateDto = new UpdateEmployeeDto
            {
                Id = 1,
                FirstName = ""
            };

            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("FirstName", "First name is required")
            };

            updateEmployeeValidatorMock
                .Setup(v => v.ValidateAsync(updateDto, default))
                .ReturnsAsync(new ValidationResult(failures));

            // Act & Assert
            await FluentActions.Invoking(() => employeeService.UpdateEmployee(updateDto))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMessage("*First name is required*");

            employeeRepoMock.Verify(r => r.UpdateEmployee(It.IsAny<Employee>()), Times.Never);
            employeeRepoMock.Verify(r => r.GetEmployeeById(It.IsAny<int>()), Times.Never);
            companyRepoMock.Verify(r => r.GetCompanyById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UpdateEmployee_ShouldThrowException_WhenEmployeeNotFound()
        {
            // Arrange
            var updateDto = new UpdateEmployeeDto
            {
                Id = 999,
                FirstName = "John",
                LastName = "Doe",
                Title = "Manager",
                CompanyId = 1
            };

            updateEmployeeValidatorMock
                .Setup(v => v.ValidateAsync(updateDto, default))
                .ReturnsAsync(new ValidationResult());

            employeeRepoMock
                .Setup(r => r.GetEmployeeById(updateDto.Id))
                .ReturnsAsync((Employee?)null);

            // Act & Assert
            await FluentActions.Invoking(() => employeeService.UpdateEmployee(updateDto))
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage($"*Employee with ID {updateDto.Id} not found*");

            employeeRepoMock.Verify(r => r.UpdateEmployee(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public async Task UpdateEmployee_ShouldThrowException_WhenCompanyNotFound()
        {
            // Arrange
            var updateDto = new UpdateEmployeeDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Title = "Manager",
                CompanyId = 999  // Non-existent Company
            };

            var existingEmployee = new Employee
            {
                Id = 1,
                Company = new Company { Id = 1, Name = "Old Co" }
            };

            updateEmployeeValidatorMock
                .Setup(v => v.ValidateAsync(updateDto, default))
                .ReturnsAsync(new ValidationResult());

            employeeRepoMock
                .Setup(r => r.GetEmployeeById(updateDto.Id))
                .ReturnsAsync(existingEmployee);

            companyRepoMock
                .Setup(r => r.GetCompanyById(updateDto.CompanyId))
                .ReturnsAsync((Company?)null);

            // Act & Assert
            await FluentActions.Invoking(() => employeeService.UpdateEmployee(updateDto))
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage($"*Company with ID {updateDto.CompanyId} not found*");

            employeeRepoMock.Verify(r => r.UpdateEmployee(It.IsAny<Employee>()), Times.Never);
        }

        // DeleteEmployee

        [Fact]
        public async Task DeleteEmployee_ShouldCallRepositoryDeleteEmployee_WithCorrectId()
        {
            // Arrange
            var id = 1;

            employeeRepoMock
                .Setup(r => r.DeleteEmployee(id))
                .Returns(Task.CompletedTask);

            // Act
            await employeeService.DeleteEmployee(id);

            // Assert
            employeeRepoMock.Verify(r => r.DeleteEmployee(id), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployee_ShouldThrowArgumentException_WhenIdIsInvalid()
        {
            var invalidId = -1;

            // Act & Assert
            await FluentActions.Invoking(() => employeeService.DeleteEmployee(invalidId))
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage("Invalid employee ID");

            employeeRepoMock.Verify(r => r.DeleteEmployee(It.IsAny<int>()), Times.Never);
        }
    }
}
