using Microsoft.AspNetCore.Mvc;
using VisitorRegistrationService;
using VisitorRegistrationService.Dtos.Appointments;

namespace VisitorRegistrationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService appointmentService;

        public AppointmentController(AppointmentService appointmentService)
        {
            this.appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await appointmentService.GetAllAppointments();

            if (!appointments.Any())
            {
                return NotFound("No appointments found");
            }
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID is not valid");
            }

            var appointment = await appointmentService.GetAppointmentById(id);

            if (appointment == null)
            {
                return NotFound($"Appointment with ID {id} was not found");
            }

            return Ok(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewAppointment([FromBody] CreateAppointmentDto createAppointmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await appointmentService.CreateNewAppointment(createAppointmentDto);

            return Ok("Appointment created successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment([FromBody] UpdateAppointmentDto updateAppointmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAppointment = await appointmentService.GetAppointmentById(updateAppointmentDto.Id);

            if (existingAppointment == null)
            {
                return NotFound($"Appointment with ID {updateAppointmentDto.Id} was not found");
            }

            await appointmentService.UpdateAppointment(updateAppointmentDto);

            return Ok("Appointment updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID is not valid");
            }

            var existingAppointment = await appointmentService.GetAppointmentById(id);

            if (existingAppointment == null)
            {
                return NotFound($"Appointment with ID {id} was not found");
            }

            await appointmentService.DeleteAppointment(id);

            return Ok("Appointment deleted successfully");
        }
    }
}
