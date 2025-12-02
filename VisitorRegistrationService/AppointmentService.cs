using FluentValidation;
using VisitorRegistrationData.Entities;
using VisitorRegistrationData.Interfaces;
using VisitorRegistrationService.Dtos.Appointments;

namespace VisitorRegistrationService
{
    public class AppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IVisitorRepository visitorRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly ICompanyRepository companyRepository;

        private readonly IValidator<CreateAppointmentDto> createAppointmentDtoValidator;
        private readonly IValidator<UpdateAppointmentDto> updateAppointmentDtoValidator;

        public AppointmentService(IAppointmentRepository appointmentRepository, IValidator<CreateAppointmentDto> createAppointmentDtoValidator,
            IValidator<UpdateAppointmentDto> updateAppointmentDtoValidator, IVisitorRepository visitorRepository, IEmployeeRepository employeeRepository,
            ICompanyRepository companyRepository)
        {
            this.appointmentRepository = appointmentRepository;
            this.createAppointmentDtoValidator = createAppointmentDtoValidator;
            this.updateAppointmentDtoValidator = updateAppointmentDtoValidator;
            this.visitorRepository = visitorRepository;
            this.employeeRepository = employeeRepository;
            this.companyRepository = companyRepository;
        }

        public async Task<List<GetAppointmentDto>> GetAllAppointments()
        {
            var appointments = await appointmentRepository.GetAllAppointments();
            return appointments.Select(a => a.ToGetDto()).ToList();
        }

        public async Task<GetAppointmentDto?> GetAppointmentById(int id)
        {
            var appointment = await appointmentRepository.GetAppointmentById(id);
            return appointment?.ToGetDto();
        }

        public async Task CreateNewAppointment(CreateAppointmentDto createAppointmentDto)
        {
            var validationResult = await createAppointmentDtoValidator.ValidateAsync(createAppointmentDto);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var visitor = await visitorRepository.GetVisitorById(createAppointmentDto.VisitorId);

            if (visitor == null)
                throw new ArgumentException("Visitor not found.");

            var employee = await employeeRepository.GetEmployeeById(createAppointmentDto.EmployeeId);

            if (employee == null)
                throw new ArgumentException("Employee not found");

            var company = await companyRepository.GetCompanyById(createAppointmentDto.CompanyId);

            if (company == null)
                throw new ArgumentException("Company not found or is deleted");

            var employeeAppointments = await appointmentRepository.GetAppointmentsByEmployeeId(createAppointmentDto.EmployeeId);

            foreach (var appointment in employeeAppointments)
            {
                if (IsOverlapping(createAppointmentDto.AppointmentStartDate, createAppointmentDto.AppointmentEndDate,
                    appointment.AppointmentStartDate, appointment.AppointmentEndDate))
                {
                    throw new ArgumentException("The employee has another appointment during this time.");
                }
            }

            var visitorAppointments = await appointmentRepository.GetAppointmentsByVisitorId(createAppointmentDto.VisitorId);

            foreach (var appointment in visitorAppointments)
            {
                if (IsOverlapping(createAppointmentDto.AppointmentStartDate, createAppointmentDto.AppointmentEndDate,
                    appointment.AppointmentStartDate, appointment.AppointmentEndDate))
                {
                    throw new ArgumentException("The visitor has another appointment during this time.");
                }
            }

            var appointmentDtoToEntity = createAppointmentDto.CreateDtoToEntity(company);

            await appointmentRepository.CreateAppointment(appointmentDtoToEntity);
        }

        public async Task UpdateAppointment(UpdateAppointmentDto updateAppointmentDto)
        {
            var validationResult = await updateAppointmentDtoValidator.ValidateAsync(updateAppointmentDto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException("Invalid appointment data");
            }

            var existingAppointment = await appointmentRepository.GetAppointmentById(updateAppointmentDto.Id);

            if (existingAppointment == null)
            {
                throw new ArgumentException("Appointment not found.");
            }

            var employee = await employeeRepository.GetEmployeeById(updateAppointmentDto.EmployeeId);

            if (employee == null)
                throw new ArgumentException("Employee not found");

            var company = await companyRepository.GetCompanyById(updateAppointmentDto.CompanyId);

            if (company == null)
                throw new ArgumentException("Company not found or is deleted");

            var employeeAppointments = await appointmentRepository.GetAppointmentsByEmployeeId(updateAppointmentDto.EmployeeId);

            foreach (var appointment in employeeAppointments)
            {
                if (appointment.Id == updateAppointmentDto.Id)
                    continue;

                if (IsOverlapping(updateAppointmentDto.AppointmentStartDate, updateAppointmentDto.AppointmentEndDate, appointment.AppointmentStartDate, appointment.AppointmentEndDate))
                {
                    throw new ArgumentException("The employee has another appointment during this time.");
                }
            }

            var visitorAppointments = await appointmentRepository.GetAppointmentsByVisitorId(existingAppointment.VisitorId);

            foreach (var appointment in visitorAppointments)
            {
                if (appointment.Id == updateAppointmentDto.Id)
                    continue;

                if (IsOverlapping(updateAppointmentDto.AppointmentStartDate, updateAppointmentDto.AppointmentEndDate, appointment.AppointmentStartDate, appointment.AppointmentEndDate))
                {
                    throw new ArgumentException("The visitor has another appointment during this time.");
                }
            }

            updateAppointmentDto.UpdateDtoToEntity(existingAppointment, company);

            await appointmentRepository.UpdateAppointment(existingAppointment);
        }

        public async Task DeleteAppointment(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid appointment ID.");

            var existing = await appointmentRepository.GetAppointmentById(id);

            if (existing == null)
                throw new ArgumentException("Appointment not found.");

            await appointmentRepository.DeleteAppointment(id);
        }

        public static bool IsOverlapping(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return !(end1 <= start2 || start1 >= end2);
        }

    }
}
