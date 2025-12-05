using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using VisitorRegistrationData.Entities;
using VisitorRegistrationData.Interfaces;
using VisitorRegistrationService;
using VisitorRegistrationService.Dtos.Appointments;
using VisitorRegistrationShared.Dtos.Appointments;

namespace VisitorRegistrationTest.AppointmentTests
{
    public class AppointmentServiceTest
    {
        private readonly Mock<IAppointmentRepository> appointmentRepositoryMock;
        private readonly Mock<ICompanyRepository> companyRepositoryMock;
        private readonly Mock<IEmployeeRepository> employeeRepositoryMock;
        private readonly Mock<IVisitorRepository> visitorRepositoryMock;

        private readonly Mock<IValidator<CreateAppointmentDto>> createAppointmentValidatorMock;
        private readonly Mock<IValidator<UpdateAppointmentDto>> updateAppointmentValidatorMock;

        private readonly AppointmentService appointmentService;

        public AppointmentServiceTest()
        {
            appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            companyRepositoryMock = new Mock<ICompanyRepository>();
            employeeRepositoryMock = new Mock<IEmployeeRepository>();
            visitorRepositoryMock = new Mock<IVisitorRepository>();

            createAppointmentValidatorMock = new Mock<IValidator<CreateAppointmentDto>>();
            updateAppointmentValidatorMock = new Mock<IValidator<UpdateAppointmentDto>>();

            appointmentService = new AppointmentService(
                appointmentRepositoryMock.Object,
                createAppointmentValidatorMock.Object,
                updateAppointmentValidatorMock.Object,
                visitorRepositoryMock.Object,
                employeeRepositoryMock.Object,
                companyRepositoryMock.Object);
        }

        // GetAllAppointments

        [Fact]
        public async Task GetAllAppointments_ShouldReturnList_WhenAppointmentsExist()
        {
            // Arrange
            var company = new Company { Id = 1, Name = "Test Company" };

            var visitor1 = new Visitor { Id = 1, Firstname = "Alice", Lastname = "Jones", Email = "alice@test.com" };
            var visitor2 = new Visitor { Id = 2, Firstname = "Bob", Lastname = "Smith", Email = "bob@test.com" };

            var employee1 = new Employee { Id = 1, FirstName = "John", LastName = "Doe", Title = "Manager", Company = company };
            var employee2 = new Employee { Id = 2, FirstName = "Jane", LastName = "Adams", Title = "Lead", Company = company };

            var mockAppointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = 1,
                    AppointmentStartDate = DateTime.Now.AddHours(1),
                    AppointmentEndDate = DateTime.Now.AddHours(2),
                    VisitorId = 1,
                    EmployeeId = 1,
                    CompanyId = 1,
                    Company = company,
                    Visitor = visitor1,
                    Employee = employee1
                },
                new Appointment
                {
                    Id = 2,
                    AppointmentStartDate = DateTime.Now.AddHours(3),
                    AppointmentEndDate = DateTime.Now.AddHours(4),
                    VisitorId = 2,
                    EmployeeId = 2,
                    CompanyId = 1,
                    Company = company,
                    Visitor = visitor2,
                    Employee = employee2
                }
            };

            appointmentRepositoryMock
                .Setup(r => r.GetAllAppointments())
                .ReturnsAsync(mockAppointments);

            // Act
            var result = await appointmentService.GetAllAppointments();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].CompanyName.Should().Be("Test Company");
            result[0].VisitorFirstname.Should().Be("Alice");
            result[0].VisitorLastname.Should().Be("Jones");
            result[0].EmployeeFirstname.Should().Be("John");
            result[0].EmployeeLastname.Should().Be("Doe");
        }

        [Fact]
        public async Task GetAllAppointments_ShouldReturnEmptyList_WhenNoAppointmentsExist()
        {
            // Arrange
            appointmentRepositoryMock
                .Setup(r => r.GetAllAppointments())
                .ReturnsAsync(new List<Appointment>());

            // Act
            var result = await appointmentService.GetAllAppointments();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        // GetAppointmentById

        [Fact]
        public async Task GetAppointmentById_ShouldReturnAppointment_WhenExists()
        {
            // Arrange
            var company = new Company { Id = 1, Name = "Tech Corp" };
            var visitor = new Visitor { Id = 1, Firstname = "Alice", Lastname = "Jones", Email = "alice@mail.com" };
            var employee = new Employee
            {
                Id = 1,
                FirstName = "Emma",
                LastName = "Brown",
                Title = "Manager",
                CompanyId = 1,
                Company = company
            };

            var appointment = new Appointment
            {
                Id = 1,
                AppointmentStartDate = DateTime.Now.AddHours(1),
                AppointmentEndDate = DateTime.Now.AddHours(2),
                VisitorId = 1,
                EmployeeId = 1,
                CompanyId = 1,
                Company = company,
                Visitor = visitor,
                Employee = employee
            };

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentById(1))
                .ReturnsAsync(appointment);

            // Act
            var result = await appointmentService.GetAppointmentById(1);

            // Assert
            result.Should().NotBeNull();
            result.VisitorFirstname.Should().Be("Alice");
            result.VisitorLastname.Should().Be("Jones");
            result.EmployeeFirstname.Should().Be("Emma");
            result.EmployeeLastname.Should().Be("Brown");
            result.CompanyName.Should().Be("Tech Corp");
        }

        [Fact]
        public async Task GetAppointmentById_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentById(999))
                .ReturnsAsync((Appointment?)null);

            // Act
            var result = await appointmentService.GetAppointmentById(999);

            // Assert
            result.Should().BeNull();
        }

        // CreateNewAppointment

        [Fact]
        public async Task CreateNewAppointment_ShouldCreateAppointment_WhenDataIsValid()
        {
            // Arrange
            var company = new Company { Id = 1, Name = "Tech Corp" };

            var visitor = new Visitor
            {
                Id = 1,
                Firstname = "Alice",
                Lastname = "Jones",
                Email = "alice@mail.com"
            };

            var employee = new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Title = "Manager",
                CompanyId = 1,
                Company = company
            };

            var createDto = new CreateAppointmentDto
            {
                AppointmentStartDate = DateTime.Now.AddHours(1),
                AppointmentEndDate = DateTime.Now.AddHours(2),
                VisitorId = 1,
                EmployeeId = 1,
                CompanyId = 1
            };

            createAppointmentValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            visitorRepositoryMock
                .Setup(r => r.GetVisitorById(createDto.VisitorId))
                .ReturnsAsync(visitor);

            employeeRepositoryMock
                .Setup(r => r.GetEmployeeById(createDto.EmployeeId))
                .ReturnsAsync(employee);

            companyRepositoryMock
                .Setup(r => r.GetCompanyById(createDto.CompanyId))
                .ReturnsAsync(company);

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentsByEmployeeId(createDto.EmployeeId))
                .ReturnsAsync(new List<Appointment>());

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentsByVisitorId(createDto.VisitorId))
                .ReturnsAsync(new List<Appointment>());

            // Act
            await appointmentService.CreateNewAppointment(createDto);

            // Assert
            appointmentRepositoryMock.Verify(
                r => r.CreateAppointment(It.Is<Appointment>(a =>
                    a.VisitorId == 1 &&
                    a.EmployeeId == 1 &&
                    a.CompanyId == 1 &&
                    a.AppointmentStartDate == createDto.AppointmentStartDate &&
                    a.AppointmentEndDate == createDto.AppointmentEndDate &&
                    a.Company == company)), Times.Once);
        }

        [Fact]
        public async Task CreateNewAppointment_ShouldThrowValidationException_WhenEndDateIsInvalid()
        {
            // Arrange
            var createDto = new CreateAppointmentDto
            {
                AppointmentStartDate = DateTime.Now,
                AppointmentEndDate = DateTime.Now.AddHours(-1),
                VisitorId = 1,
                EmployeeId = 1,
                CompanyId = 1
            };

            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("AppointmentStartDate", "Start date must be before end date.")
            };

            createAppointmentValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult(failures));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                appointmentService.CreateNewAppointment(createDto));

            appointmentRepositoryMock.Verify(r => r.CreateAppointment(It.IsAny<Appointment>()), Times.Never);
        }

        [Fact]
        public async Task CreateNewAppointment_ShouldThrowArgumentException_WhenVisitorNotFound()
        {
            // Arrange
            var createDto = new CreateAppointmentDto
            {
                AppointmentStartDate = DateTime.Now.AddHours(1),
                AppointmentEndDate = DateTime.Now.AddHours(2),
                VisitorId = 99,
                EmployeeId = 1,
                CompanyId = 1
            };

            createAppointmentValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            visitorRepositoryMock
                .Setup(r => r.GetVisitorById(createDto.VisitorId))
                .ReturnsAsync((Visitor?)null);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                appointmentService.CreateNewAppointment(createDto));

            appointmentRepositoryMock.Verify(r => r.CreateAppointment(It.IsAny<Appointment>()), Times.Never);
        }

        [Fact]
        public async Task CreateNewAppointment_ShouldThrowArgumentException_WhenEmployeeNotFound()
        {
            // Arrange
            var createDto = new CreateAppointmentDto
            {
                AppointmentStartDate = DateTime.Now.AddHours(1),
                AppointmentEndDate = DateTime.Now.AddHours(2),
                VisitorId = 1,
                EmployeeId = 999,
                CompanyId = 1
            };

            createAppointmentValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            visitorRepositoryMock
                .Setup(r => r.GetVisitorById(1))
                .ReturnsAsync(new Visitor
                {
                    Id = 2,
                    Firstname = "Alice",
                    Lastname = "Jones",
                    Email = "alicejones@test.com"
                });

            employeeRepositoryMock
                .Setup(r => r.GetEmployeeById(createDto.EmployeeId))
                .ReturnsAsync((Employee?)null);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                appointmentService.CreateNewAppointment(createDto));

            appointmentRepositoryMock.Verify(r => r.CreateAppointment(It.IsAny<Appointment>()), Times.Never);
        }

        [Fact]
        public async Task CreateNewAppointment_ShouldThrowArgumentException_WhenCompanyNotFound()
        {
            // Arrange
            var createDto = new CreateAppointmentDto
            {
                AppointmentStartDate = DateTime.Now.AddHours(1),
                AppointmentEndDate = DateTime.Now.AddHours(2),
                VisitorId = 1,
                EmployeeId = 1,
                CompanyId = 999
            };

            createAppointmentValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            visitorRepositoryMock.Setup(r => r.GetVisitorById(1)).ReturnsAsync(new Visitor
            {
                Id = 2,
                Firstname = "Alice",
                Lastname = "Jones",
                Email = "alicejones@test.com"
            });

            employeeRepositoryMock.Setup(r => r.GetEmployeeById(1)).ReturnsAsync(new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Title = "Manager",
                CompanyId = 1,
                Company = new Company { Id = 1, Name = "Tech Corp" }
            });

            companyRepositoryMock
                .Setup(r => r.GetCompanyById(createDto.CompanyId))
                .ReturnsAsync((Company?)null);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                appointmentService.CreateNewAppointment(createDto));

            appointmentRepositoryMock.Verify(r => r.CreateAppointment(It.IsAny<Appointment>()), Times.Never);
        }

        [Fact]
        public async Task CreateNewAppointment_ShouldThrowArgumentException_WhenEmployeeHasOverlappingAppointment()
        {
            // Arrange
            var createDto = new CreateAppointmentDto
            {
                AppointmentStartDate = DateTime.Now.AddHours(1),
                AppointmentEndDate = DateTime.Now.AddHours(2),
                VisitorId = 1,
                EmployeeId = 1,
                CompanyId = 1
            };

            createAppointmentValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            visitorRepositoryMock.Setup(r => r.GetVisitorById(1)).ReturnsAsync(new Visitor
            {
                Id = 2,
                Firstname = "Alice",
                Lastname = "Jones",
                Email = "alicejones@test.com"
            });

            employeeRepositoryMock.Setup(r => r.GetEmployeeById(1)).ReturnsAsync(new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Title = "Manager",
                CompanyId = 1,
                Company = new Company { Id = 1, Name = "Tech Corp" }
            });

            companyRepositoryMock.Setup(r => r.GetCompanyById(1)).ReturnsAsync(new Company());

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentsByEmployeeId(1))
                .ReturnsAsync(new List<Appointment>
                {
                    new Appointment
                    {
                        AppointmentStartDate = createDto.AppointmentStartDate.AddMinutes(10),
                        AppointmentEndDate = createDto.AppointmentEndDate.AddMinutes(10),
                        EmployeeId = 1,
                        VisitorId = 99,
                        CompanyId = 1,
                        Company = new Company { Id = 1, Name = "Tech Corp"}
                    }
                });

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentsByVisitorId(1))
                .ReturnsAsync(new List<Appointment>());

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                appointmentService.CreateNewAppointment(createDto));

            appointmentRepositoryMock.Verify(r => r.CreateAppointment(It.IsAny<Appointment>()), Times.Never);
        }

        [Fact]
        public async Task CreateNewAppointment_ShouldThrowArgumentException_WhenVisitorHasOverlappingAppointment()
        {
            // Arrange
            var createDto = new CreateAppointmentDto
            {
                AppointmentStartDate = DateTime.Now.AddHours(1),
                AppointmentEndDate = DateTime.Now.AddHours(2),
                VisitorId = 1,
                EmployeeId = 1,
                CompanyId = 1
            };

            createAppointmentValidatorMock
                .Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            visitorRepositoryMock.Setup(r => r.GetVisitorById(1))
                .ReturnsAsync(new Visitor
                {
                    Id = 1,
                    Firstname = "Alice",
                    Lastname = "Jones",
                    Email = "alice@test.com"
                });

            employeeRepositoryMock.Setup(r => r.GetEmployeeById(1))
                .ReturnsAsync(new Employee
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Title = "Manager",
                    CompanyId = 1,
                    Company = new Company { Id = 1, Name = "Tech Corp" }
                });

            companyRepositoryMock.Setup(r => r.GetCompanyById(1))
                .ReturnsAsync(new Company { Id = 1, Name = "Tech Corp" });

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentsByEmployeeId(1))
                .ReturnsAsync(new List<Appointment>());

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentsByVisitorId(1))
                .ReturnsAsync(new List<Appointment>
                {
                    new Appointment
                    {
                        AppointmentStartDate = createDto.AppointmentStartDate.AddMinutes(5),
                        AppointmentEndDate = createDto.AppointmentEndDate.AddMinutes(5),
                        VisitorId = 1,
                        EmployeeId = 99,
                        CompanyId = 1,
                        Company = new Company { Id = 1, Name = "Tech Corp" }
                    }
                });

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                appointmentService.CreateNewAppointment(createDto));

            appointmentRepositoryMock.Verify(
                r => r.CreateAppointment(It.IsAny<Appointment>()),
                Times.Never);
        }

        // UpdateAppointment

        [Fact]
        public async Task UpdateAppointment_ShouldUpdateAppointment_WhenDataIsValid()
        {
            // Arrange
            var company = new Company { Id = 1, Name = "Tech Corp" };

            var existingAppointment = new Appointment
            {
                Id = 1,
                AppointmentStartDate = DateTime.Now.AddHours(10),
                AppointmentEndDate = DateTime.Now.AddHours(11),
                VisitorId = 5,
                EmployeeId = 1,
                CompanyId = 1,
                Company = company
            };

            var updateDto = new UpdateAppointmentDto
            {
                Id = 1,
                AppointmentStartDate = DateTime.Now.AddHours(1),
                AppointmentEndDate = DateTime.Now.AddHours(2),
                EmployeeId = 1,
                CompanyId = 1
            };

            updateAppointmentValidatorMock
                .Setup(v => v.ValidateAsync(updateDto, default))
                .ReturnsAsync(new ValidationResult());

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentById(updateDto.Id))
                .ReturnsAsync(existingAppointment);

            employeeRepositoryMock
                .Setup(r => r.GetEmployeeById(updateDto.EmployeeId))
                .ReturnsAsync(new Employee
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Title = "Manager",
                    Company = company
                });

            companyRepositoryMock
                .Setup(r => r.GetCompanyById(updateDto.CompanyId))
                .ReturnsAsync(company);

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentsByEmployeeId(updateDto.EmployeeId))
                .ReturnsAsync(new List<Appointment>
                {
                    new Appointment
                    {
                        Id = 1,
                        AppointmentStartDate = DateTime.Now,
                        AppointmentEndDate = DateTime.Now.AddHours(1),
                        CompanyId = 1,
                        Company = new Company { Id = 1, Name = "Tech Corp" },
                        VisitorId = 999,
                        EmployeeId = 1
                    }
                });

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentsByVisitorId(existingAppointment.VisitorId))
                .ReturnsAsync(new List<Appointment>
                {
                    new Appointment
                    {
                        Id = 1,
                        AppointmentStartDate = DateTime.Now,
                        AppointmentEndDate = DateTime.Now.AddHours(1),
                        CompanyId = 1,
                        Company = new Company { Id = 1, Name = "Tech Corp" },
                        VisitorId = 999,
                        EmployeeId = 1
                    }

                });

            // Act
            await appointmentService.UpdateAppointment(updateDto);

            // Assert
            appointmentRepositoryMock.Verify(r =>
                r.UpdateAppointment(It.Is<Appointment>(a =>
                    a.Id == 1 &&
                    a.AppointmentStartDate == updateDto.AppointmentStartDate &&
                    a.AppointmentEndDate == updateDto.AppointmentEndDate &&
                    a.EmployeeId == updateDto.EmployeeId &&
                    a.CompanyId == updateDto.CompanyId &&
                    a.Company == company
                )), Times.Once);
        }

        [Fact]
        public async Task UpdateAppointment_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var updateDto = new UpdateAppointmentDto
            {
                Id = 1,
                AppointmentStartDate = DateTime.Now,
                AppointmentEndDate = DateTime.Now.AddHours(-1), 
                EmployeeId = 1,
                CompanyId = 1
            };

            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("AppointmentStartDate", "Start date must be before end date.")
            };

            updateAppointmentValidatorMock
                .Setup(v => v.ValidateAsync(updateDto, default))
                .ReturnsAsync(new ValidationResult(failures));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                appointmentService.UpdateAppointment(updateDto));

            appointmentRepositoryMock.Verify(r => r.UpdateAppointment(It.IsAny<Appointment>()), Times.Never);
            appointmentRepositoryMock.Verify(r => r.GetAppointmentById(It.IsAny<int>()), Times.Never);
            employeeRepositoryMock.Verify(r => r.GetEmployeeById(It.IsAny<int>()), Times.Never);
            visitorRepositoryMock.Verify(r => r.GetVisitorById(It.IsAny<int>()), Times.Never);
            companyRepositoryMock.Verify(r => r.GetCompanyById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAppointment_ShouldThrowArgumentException_WhenAppointmentNotFound()
        {
            // Arrange
            var updateDto = new UpdateAppointmentDto
            {
                Id = 99,
                AppointmentStartDate = DateTime.Now.AddHours(1),
                AppointmentEndDate = DateTime.Now.AddHours(2),
                EmployeeId = 1,
                CompanyId = 1
            };

            updateAppointmentValidatorMock
                .Setup(v => v.ValidateAsync(updateDto, default))
                .ReturnsAsync(new ValidationResult());

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentById(updateDto.Id))
                .ReturnsAsync((Appointment?)null);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                appointmentService.UpdateAppointment(updateDto));

            appointmentRepositoryMock.Verify(r => r.UpdateAppointment(It.IsAny<Appointment>()), Times.Never);

            updateAppointmentValidatorMock.Verify(v => v.ValidateAsync(updateDto, default), Times.Once);
            appointmentRepositoryMock.Verify(r => r.GetAppointmentById(updateDto.Id), Times.Once);

            companyRepositoryMock.Verify(r => r.GetCompanyById(It.IsAny<int>()), Times.Never);
            employeeRepositoryMock.Verify(r => r.GetEmployeeById(It.IsAny<int>()), Times.Never);
            visitorRepositoryMock.Verify(r => r.GetVisitorById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAppointment_ShouldThrowArgumentException_WhenEmployeeHasOverlappingAppointment()
        {
            // Arrange
            var company = new Company { Id = 1, Name = "Tech Corp" };

            var existingAppointment = new Appointment
            {
                Id = 1,
                AppointmentStartDate = DateTime.Now.AddHours(10),
                AppointmentEndDate = DateTime.Now.AddHours(11),
                VisitorId = 1,
                EmployeeId = 1,
                CompanyId = 1,
                Company = company
            };

            var updateDto = new UpdateAppointmentDto
            {
                Id = 1,
                AppointmentStartDate = DateTime.Now.AddHours(1),
                AppointmentEndDate = DateTime.Now.AddHours(2),
                EmployeeId = 1,
                CompanyId = 1
            };

            updateAppointmentValidatorMock
                .Setup(v => v.ValidateAsync(updateDto, default))
                .ReturnsAsync(new ValidationResult());

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentById(1))
                .ReturnsAsync(existingAppointment);

            employeeRepositoryMock
                .Setup(r => r.GetEmployeeById(1))
                .ReturnsAsync(new Employee { Id = 1, FirstName = "John", LastName = "Doe", Company = company });

            companyRepositoryMock
                .Setup(r => r.GetCompanyById(1))
                .ReturnsAsync(company);

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentsByEmployeeId(1))
                .ReturnsAsync(new List<Appointment>
                {
                    new Appointment
                    {
                        Id = 2,
                        AppointmentStartDate = updateDto.AppointmentStartDate.AddMinutes(10),
                        AppointmentEndDate = updateDto.AppointmentEndDate.AddMinutes(10),
                        EmployeeId = 1,
                        CompanyId = 1,
                        Company = company,
                        VisitorId = 99
                    }
                });

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentsByVisitorId(existingAppointment.VisitorId))
                .ReturnsAsync(new List<Appointment>());

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                appointmentService.UpdateAppointment(updateDto));
        }

        [Fact]
        public async Task UpdateAppointment_ShouldThrowArgumentException_WhenVisitorHasOverlappingAppointment()
        {
            // Arrange
            var company = new Company { Id = 1, Name = "Tech Corp" };

            var existingAppointment = new Appointment
            {
                Id = 1,
                AppointmentStartDate = DateTime.Now.AddHours(10),
                AppointmentEndDate = DateTime.Now.AddHours(11),
                VisitorId = 5,
                EmployeeId = 1,
                CompanyId = 1,
                Company = company
            };

            var updateDto = new UpdateAppointmentDto
            {
                Id = 1,
                AppointmentStartDate = DateTime.Now.AddHours(1),
                AppointmentEndDate = DateTime.Now.AddHours(2),
                EmployeeId = 1,
                CompanyId = 1
            };

            updateAppointmentValidatorMock
                .Setup(v => v.ValidateAsync(updateDto, default))
                .ReturnsAsync(new ValidationResult());

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentById(1))
                .ReturnsAsync(existingAppointment);

            employeeRepositoryMock
                .Setup(r => r.GetEmployeeById(1))
                .ReturnsAsync(new Employee { Id = 1, FirstName = "John", LastName = "Doe", Company = company });

            companyRepositoryMock
                .Setup(r => r.GetCompanyById(1))
                .ReturnsAsync(company);

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentsByVisitorId(existingAppointment.VisitorId))
                .ReturnsAsync(new List<Appointment>
                {
                    new Appointment
                    {
                        Id = 2,
                        AppointmentStartDate = updateDto.AppointmentStartDate.AddMinutes(5),
                        AppointmentEndDate = updateDto.AppointmentEndDate.AddMinutes(5),
                        VisitorId = 5,
                        EmployeeId = 99,
                        CompanyId = 1,
                        Company = company
                    }
                });

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentsByEmployeeId(1))
                .ReturnsAsync(new List<Appointment>());

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                appointmentService.UpdateAppointment(updateDto));
        }

        // DeleteAppointment

        [Fact]
        public async Task DeleteAppointment_ShouldDelete_WhenIdIsValid()
        {
            // Arrange
            var existingAppointment = new Appointment
            {
                Id = 1,
                AppointmentStartDate = DateTime.Now,
                AppointmentEndDate = DateTime.Now.AddHours(1),
                VisitorId = 1,
                EmployeeId = 1,
                CompanyId = 1,
                Company = new Company { Id = 1, Name = "Tech Corp" }
            };

            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentById(1))
                .ReturnsAsync(existingAppointment);

            // Act
            await appointmentService.DeleteAppointment(1);

            // Assert
            appointmentRepositoryMock.Verify(r => r.DeleteAppointment(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAppointment_ShouldThrowArgumentException_WhenIdIsInvalid()
        {
            // Arrange
            int invalidId = 0;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                appointmentService.DeleteAppointment(invalidId));

            appointmentRepositoryMock.Verify(r => r.DeleteAppointment(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAppointment_ShouldThrowArgumentException_WhenAppointmentNotFound()
        {
            // Arrange
            appointmentRepositoryMock
                .Setup(r => r.GetAppointmentById(99))
                .ReturnsAsync((Appointment?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                appointmentService.DeleteAppointment(99));

            appointmentRepositoryMock.Verify(r => r.DeleteAppointment(It.IsAny<int>()), Times.Never);
        }

    }
}
