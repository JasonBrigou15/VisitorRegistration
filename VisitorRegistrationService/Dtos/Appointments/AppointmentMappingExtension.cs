using VisitorRegistrationShared.Dtos.Appointments;

namespace VisitorRegistrationService.Dtos.Appointments
{
    public static class AppointmentMappingExtension
    {
        public static GetAppointmentDto ToGetDto(this VisitorRegistrationData.Entities.Appointment appointment)
        {
            return new GetAppointmentDto
            {
                AppointmentStartDate = appointment.AppointmentStartDate,
                AppointmentEndDate = appointment.AppointmentEndDate,
                VisitorFirstname = appointment.Visitor.Firstname,
                VisitorLastname = appointment.Visitor.Lastname,
                EmployeeFirstname = appointment.Employee.FirstName,
                EmployeeLastname = appointment.Employee.LastName,
                CompanyName = appointment.Company.Name
            };
        }

        public static VisitorRegistrationData.Entities.Appointment CreateDtoToEntity(this CreateAppointmentDto createAppointmentDto, VisitorRegistrationData.Entities.Company company)
        {
            var appointment = new VisitorRegistrationData.Entities.Appointment
            {
                AppointmentStartDate = createAppointmentDto.AppointmentStartDate,
                AppointmentEndDate = createAppointmentDto.AppointmentEndDate,
                VisitorId = createAppointmentDto.VisitorId,
                EmployeeId = createAppointmentDto.EmployeeId,
                CompanyId = createAppointmentDto.CompanyId,
                Company = company
            };

            return appointment;
        }

        public static void UpdateDtoToEntity(this UpdateAppointmentDto updateAppointmentDto, VisitorRegistrationData.Entities.Appointment appointment, 
            VisitorRegistrationData.Entities.Company company)
        {
            appointment.AppointmentStartDate = updateAppointmentDto.AppointmentStartDate;
            appointment.AppointmentEndDate = updateAppointmentDto.AppointmentEndDate;
            appointment.EmployeeId = updateAppointmentDto.EmployeeId;
            appointment.CompanyId = updateAppointmentDto.CompanyId;
            appointment.Company = company;
        }
    }
}
